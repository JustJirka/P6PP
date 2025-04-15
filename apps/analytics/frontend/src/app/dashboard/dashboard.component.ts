import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface DataItem {
  date: string;
  value: number;
}

interface ChartItem {
  label: string;
  value: number;
}

interface RoomData {
  roomId: string;
  roomName: string;
  occupancy: DataItem[];
}

interface User {
  id: number;
  roleId: number;
  state: string;
  sex: number;
  weight: number;
  height: number;
  birthDate: string;
  createdAt: string;
  lastUpdated: string;
}

type TimeRange = 'all' | 'week' | 'month' | 'year';
type AgeGroup = 'all' | '15-18' | '18-26' | '26-30' | '30-40' | '40-50' | '50-65' | '65+';
type UserRole = 'all' | 'user' | 'trainer';

const FINANCES_DATA: DataItem[] = [
  { date: '2023-01', value: 1250 },
  { date: '2023-02', value: 1960 },
  { date: '2023-03', value: 1580 },
  { date: '2023-04', value: 2130 },
  { date: '2023-05', value: 2470 },
];

const RESERVATIONS_DATA: DataItem[] = [
  { date: '2023-01', value: 156 },
  { date: '2023-02', value: 189 },
  { date: '2023-03', value: 172 },
  { date: '2023-04', value: 210 },
  { date: '2023-05', value: 243 },
];

const ROOMS_DATA: RoomData[] = [
  {
    roomId: 'room1',
    roomName: 'Fitness Studio A',
    occupancy: [
      { date: '2023-01', value: 65 },
      { date: '2023-02', value: 78 },
      { date: '2023-03', value: 72 },
      { date: '2023-04', value: 85 },
      { date: '2023-05', value: 90 },
    ],
  },
  {
    roomId: 'room2',
    roomName: 'Yoga Studio',
    occupancy: [
      { date: '2023-01', value: 45 },
      { date: '2023-02', value: 52 },
      { date: '2023-03', value: 60 },
      { date: '2023-04', value: 75 },
      { date: '2023-05', value: 82 },
    ],
  },
  {
    roomId: 'room3',
    roomName: 'Cardio Zone',
    occupancy: [
      { date: '2023-01', value: 80 },
      { date: '2023-02', value: 85 },
      { date: '2023-03', value: 82 },
      { date: '2023-04', value: 90 },
      { date: '2023-05', value: 95 },
    ],
  },
];

const generateOverviewData = (): DataItem[] => {
  const result: DataItem[] = [];
  const months = ['2023-01', '2023-02', '2023-03', '2023-04', '2023-05'];

  months.forEach((month) => {
    const financeValue =
      FINANCES_DATA.find((item) => item.date === month)?.value || 0;

    const reservationsValue =
      RESERVATIONS_DATA.find((item) => item.date === month)?.value || 0;

    const roomOccupancies = ROOMS_DATA.flatMap((room) =>
      room.occupancy.filter((item) => item.date === month)
    );
    const avgOccupancy =
      roomOccupancies.length > 0
        ? roomOccupancies.reduce((sum, item) => sum + item.value, 0) /
          roomOccupancies.length
        : 0;

    const totalValue =
      financeValue * 0.6 +
      reservationsValue * 50 * 0.3 +
      avgOccupancy * 100 * 0.1;

    result.push({ date: month, value: Math.round(totalValue) });
  });

  return result;
};

const OVERVIEW_DATA = generateOverviewData();

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  activeTab:
    | 'overview'
    | 'finances'
    | 'reservations'
    | 'rooms'
    | 'users'
    | 'trainers' = 'overview';
  viewMode: 'chart' | 'table' = 'chart';
  selectedRoom: string | null = null;
  
  userStateFilter: 'all' | 'active' | 'inactive' = 'all';
  userSexFilter: 'all' | 'male' | 'female' = 'all';
  userRoleFilter: UserRole = 'all';
  userAgeGroupFilter: AgeGroup = 'all';
  userRegistrationTimeFilter: TimeRange = 'all';
  userLastUpdatedTimeFilter: TimeRange = 'all';

  overviewData = OVERVIEW_DATA;
  financesData = FINANCES_DATA;
  reservationsData = RESERVATIONS_DATA;
  roomsData = ROOMS_DATA;
  usersData: DataItem[] = [];
  usersTableData: User[] = [];
  trainersData: DataItem[] = [];
  
  users: User[] = [];
  isUsersLoading = false;
  usersError: string | null = null;

  yAxisValues: number[] = [];

  roleChartData: ChartItem[] = [];
  statusChartData: ChartItem[] = [];
  genderChartData: ChartItem[] = [];
  ageGroupChartData: ChartItem[] = [];
  registrationChartData: ChartItem[] = [];
  
  roleChartYAxis: number[] = [];
  statusChartYAxis: number[] = [];
  genderChartYAxis: number[] = [];
  ageGroupChartYAxis: number[] = [];
  registrationChartYAxis: number[] = [];

  constructor(private http: HttpClient) {}

  loadUsers() {
    this.isUsersLoading = true;
    this.usersError = null;
    
    this.http.get<User[]>('http://localhost:8006/api/Users', { withCredentials: false })
      .subscribe({
        next: (data) => {
          this.users = data;
          this.processUserData();
          this.generateChartData();
          this.isUsersLoading = false;
        },
        error: (error) => {
          console.error('Chyba při načítání uživatelů:', error);
          this.usersError = 'Failed to load data from API.';
          this.isUsersLoading = false;
        }
      });
  }

  processUserData() {
    if (!this.users || this.users.length === 0) {
      this.usersData = [];
      this.usersTableData = [];
      return;
    }

    let filteredUsers = this.applyAllUserFilters(this.users);
    
    this.usersTableData = filteredUsers;
    
    const usersByMonth = filteredUsers.reduce((acc, user) => {
      const date = new Date(user.createdAt);
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
      
      if (!acc[monthKey]) {
        acc[monthKey] = 0;
      }
      
      acc[monthKey]++;
      return acc;
    }, {} as Record<string, number>);
    
    this.usersData = Object.entries(usersByMonth)
      .map(([date, value]) => ({ date, value }))
      .sort((a, b) => a.date.localeCompare(b.date));
    
    this.updateYAxisValues();
    this.generateChartData();
  }

  applyAllUserFilters(users: User[]): User[] {
    let filteredUsers = [...users];
    
    if (this.userRoleFilter !== 'all') {
      const roleIdToFilter = this.userRoleFilter === 'user' ? 0 : 1;
      filteredUsers = filteredUsers.filter(user => user.roleId === roleIdToFilter);
    }
    
    if (this.userStateFilter !== 'all') {
      const stateToFilter = this.userStateFilter === 'active' ? 'active' : 'inactive';
      filteredUsers = filteredUsers.filter(user => user.state === stateToFilter);
    }
    
    if (this.userSexFilter !== 'all') {
      const sexToFilter = this.userSexFilter === 'male' ? 0 : 1;
      filteredUsers = filteredUsers.filter(user => user.sex === sexToFilter);
    }
    
    if (this.userAgeGroupFilter !== 'all') {
      filteredUsers = filteredUsers.filter(user => this.isUserInAgeGroup(user, this.userAgeGroupFilter));
    }
    
    if (this.userRegistrationTimeFilter !== 'all') {
      filteredUsers = filteredUsers.filter(user => 
        this.isDateInTimeRange(new Date(user.createdAt), this.userRegistrationTimeFilter)
      );
    }
    
    if (this.userLastUpdatedTimeFilter !== 'all') {
      filteredUsers = filteredUsers.filter(user => 
        this.isDateInTimeRange(new Date(user.lastUpdated), this.userLastUpdatedTimeFilter)
      );
    }
    
    return filteredUsers;
  }

  isDateInTimeRange(date: Date, range: TimeRange): boolean {
    if (range === 'all') return true;
    
    const now = new Date();
    
    if (range === 'week') {
      const oneWeekAgo = new Date();
      oneWeekAgo.setDate(now.getDate() - 7);
      return date >= oneWeekAgo;
    }
    
    if (range === 'month') {
      const oneMonthAgo = new Date();
      oneMonthAgo.setDate(now.getDate() - 30);
      return date >= oneMonthAgo;
    }
    
    if (range === 'year') {
      const oneYearAgo = new Date();
      oneYearAgo.setFullYear(now.getFullYear() - 1);
      return date >= oneYearAgo;
    }
    
    return false;
  }

  isUserInAgeGroup(user: User, ageGroup: AgeGroup): boolean {
    if (ageGroup === 'all') return true;
    
    const birthDate = new Date(user.birthDate);
    const today = new Date();
    
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    
    switch(ageGroup) {
      case '15-18': return age >= 15 && age <= 18;
      case '18-26': return age > 18 && age <= 26;
      case '26-30': return age > 26 && age <= 30;
      case '30-40': return age > 30 && age <= 40;
      case '40-50': return age > 40 && age <= 50;
      case '50-65': return age > 50 && age <= 65;
      case '65+': return age > 65;
      default: return false;
    }
  }

  calculateBMI(weight: number, height: number): number {
    const heightInMeters = height / 100;
    return weight / (heightInMeters * heightInMeters);
  }

  formatBMI(bmi: number): string {
    const roundedBMI = Math.round(bmi * 10) / 10;
    
    let classification = '';
    if (bmi < 18.5) classification = '(Underweight)';
    else if (bmi < 25) classification = '(Normal)';
    else if (bmi < 30) classification = '(Overweight)';
    else classification = '(Obese)';
    
    return `${roundedBMI} ${classification}`;
  }

  getBMIClass(bmi: number): string {
    if (bmi < 18.5) return 'bmi-underweight';
    else if (bmi < 25) return 'bmi-normal';
    else if (bmi < 30) return 'bmi-overweight';
    else return 'bmi-obese';
  }

  calculateAge(birthDate: string): number {
    const birth = new Date(birthDate);
    const today = new Date();
    
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    
    return age;
  }

  setUserRoleFilter(filter: UserRole) {
    this.userRoleFilter = filter;
    this.processUserData();
  }

  setUserStateFilter(filter: 'all' | 'active' | 'inactive') {
    this.userStateFilter = filter;
    this.processUserData();
  }
  
  setUserSexFilter(filter: 'all' | 'male' | 'female') {
    this.userSexFilter = filter;
    this.processUserData();
  }
  
  setUserAgeGroupFilter(filter: AgeGroup) {
    this.userAgeGroupFilter = filter;
    this.processUserData();
  }
  
  setUserRegistrationTimeFilter(filter: TimeRange) {
    this.userRegistrationTimeFilter = filter;
    this.processUserData();
  }
  
  setUserLastUpdatedTimeFilter(filter: TimeRange) {
    this.userLastUpdatedTimeFilter = filter;
    this.processUserData();
  }
  
  onUserRoleFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.setUserRoleFilter(select.value as UserRole);
  }
  
  onUserStateFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.setUserStateFilter(select.value as 'all' | 'active' | 'inactive');
  }
  
  onUserSexFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.setUserSexFilter(select.value as 'all' | 'male' | 'female');
  }
  
  onUserAgeGroupFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.setUserAgeGroupFilter(select.value as AgeGroup);
  }
  
  onUserRegistrationTimeFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.setUserRegistrationTimeFilter(select.value as TimeRange);
  }
  
  onUserLastUpdatedTimeFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.setUserLastUpdatedTimeFilter(select.value as TimeRange);
  }

  formatRole(roleId: number): string {
    return roleId === 0 ? 'User' : 'Trainer';
  }

  formatSex(sex: number): string {
    return sex === 0 ? 'Male' : 'Female';
  }

  formatState(state: string): string {
    return state.charAt(0).toUpperCase() + state.slice(1);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }

  get activeData(): DataItem[] {
    switch (this.activeTab) {
      case 'overview':
        return this.overviewData;
      case 'finances':
        return this.financesData;
      case 'reservations':
        return this.reservationsData;
      case 'rooms':
        if (this.selectedRoom) {
          const room = this.roomsData.find(
            (r) => r.roomId === this.selectedRoom
          );
          return room ? room.occupancy : [];
        }
        return this.getAverageRoomOccupancy();
      case 'users':
        return this.usersData;
      case 'trainers':
        return this.trainersData;
      default:
        return this.overviewData;
    }
  }

  get activeTitle(): string {
    switch (this.activeTab) {
      case 'overview':
        return 'Business Overview';
      case 'finances':
        return 'Financial Analytics';
      case 'reservations':
        return 'Reservation Statistics';
      case 'rooms':
        if (this.selectedRoom) {
          const room = this.roomsData.find(
            (r) => r.roomId === this.selectedRoom
          );
          return room ? `${room.roomName} Occupancy` : 'Room Occupancy';
        }
        return 'Average Room Occupancy';
      case 'users':
        return 'User Statistics';
      case 'trainers':
        return 'Trainer Analytics';
      default:
        return 'Dashboard';
    }
  }

  get valueLabel(): string {
    switch (this.activeTab) {
      case 'overview':
        return 'Business Score';
      case 'finances':
        return 'Revenue ($)';
      case 'reservations':
        return 'Number of Reservations';
      case 'rooms':
        return 'Occupancy (%)';
      case 'users':
        return 'Number of Users';
      case 'trainers':
        return 'Trainer Sessions';
      default:
        return 'Value';
    }
  }

  getAverageRoomOccupancy(): DataItem[] {
    const months = ['2023-01', '2023-02', '2023-03', '2023-04', '2023-05'];
    return months.map((month) => {
      const occupancies = this.roomsData.flatMap((room) =>
        room.occupancy.filter((item) => item.date === month)
      );

      const avgValue =
        occupancies.length > 0
          ? occupancies.reduce((sum, item) => sum + item.value, 0) /
            occupancies.length
          : 0;

      return { date: month, value: Math.round(avgValue) };
    });
  }

  ngOnInit() {
    this.loadUsers();
    this.updateYAxisValues();
  }

  setActiveTab(
    tab:
      | 'overview'
      | 'finances'
      | 'reservations'
      | 'rooms'
      | 'users'
      | 'trainers'
  ) {
    this.activeTab = tab;
    if (tab !== 'rooms') {
      this.selectedRoom = null;
    }
    if (tab === 'users') {
      this.userStateFilter = 'all';
      this.userSexFilter = 'all';
      this.userRoleFilter = 'all';
      this.userAgeGroupFilter = 'all';
      this.userRegistrationTimeFilter = 'all';
      this.userLastUpdatedTimeFilter = 'all';
      this.processUserData();
    }
    this.updateYAxisValues();
  }

  selectRoom(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.selectedRoom = select.value || null;
    this.updateYAxisValues();
  }

  toggleView() {
    this.viewMode = this.viewMode === 'chart' ? 'table' : 'chart';
  }

  updateYAxisValues() {
    const data = this.activeData;
    if (data.length === 0) {
      this.yAxisValues = [0];
      return;
    }

    const maxValue = Math.max(...data.map((item) => item.value));
    const steps = 5; // počet kroků na y-ose

    const roundedMax = this.roundToNiceNumber(maxValue);
    const step = roundedMax / steps;

    this.yAxisValues = Array.from({ length: steps + 1 }, (_, i) => {
      return Math.round(roundedMax - i * step);
    });
  }

  roundToNiceNumber(value: number): number {
    if (value < 10) return Math.ceil(value);

    if (value < 100) return Math.ceil(value / 10) * 10;

    if (value < 1000) return Math.ceil(value / 100) * 100;

    if (value < 10000) return Math.ceil(value / 1000) * 1000;

    return Math.ceil(value / 10000) * 10000;
  }

  getBarHeight(value: number, maxValue?: number): number {
    const maxAxisValue = maxValue ?? this.yAxisValues[0];
    if (maxAxisValue === 0) return 0;

    return (value / maxAxisValue) * 250;
  }

  getLatestFinanceValue(): number {
    if (this.financesData.length === 0) return 0;
    return this.financesData[this.financesData.length - 1].value;
  }

  getTotalFinanceValue(): number {
    return this.financesData.reduce((sum, item) => sum + item.value, 0);
  }

  getLatestReservationsValue(): number {
    if (this.reservationsData.length === 0) return 0;
    return this.reservationsData[this.reservationsData.length - 1].value;
  }

  getTotalReservationsValue(): number {
    return this.reservationsData.reduce((sum, item) => sum + item.value, 0);
  }

  getAverageRoomOccupancyValue(): number {
    const allOccupancies = this.roomsData.flatMap((room) => room.occupancy);
    if (allOccupancies.length === 0) return 0;

    const total = allOccupancies.reduce((sum, item) => sum + item.value, 0);
    return Math.round(total / allOccupancies.length);
  }

  getHighestOccupancyRoom(): { name: string; value: number } {
    if (this.roomsData.length === 0) {
      return { name: 'None', value: 0 };
    }

    const roomAverages = this.roomsData.map((room) => {
      const total = room.occupancy.reduce((sum, item) => sum + item.value, 0);
      const average = Math.round(total / room.occupancy.length);
      return {
        name: room.roomName,
        value: average,
      };
    });

    return roomAverages.reduce(
      (highest, current) => (current.value > highest.value ? current : highest),
      { name: 'None', value: 0 }
    );
  }
  
  getTotalUsersCount(): number {
    return this.users.length;
  }
  
  getActiveUsersCount(): number {
    return this.users.filter(user => user.state === 'active').length;
  }

  generateChartData() {
    if (!this.users || this.users.length === 0) {
      return;
    }

    const filteredUsers = this.applyAllUserFilters(this.users);
    
    this.generateRoleChartData(filteredUsers);
    
    this.generateStatusChartData(filteredUsers);
    
    this.generateGenderChartData(filteredUsers);
    
    this.generateAgeGroupChartData(filteredUsers);
    
    this.generateRegistrationChartData(filteredUsers);
  }

  generateRoleChartData(users: User[]) {
    const roleCount = users.reduce((acc, user) => {
      const role = user.roleId === 0 ? 'User' : 'Trainer';
      if (!acc[role]) {
        acc[role] = 0;
      }
      acc[role]++;
      return acc;
    }, {} as Record<string, number>);
    
    this.roleChartData = Object.entries(roleCount)
      .map(([label, value]) => ({ label, value }))
      .sort((a, b) => b.value - a.value);
    
    this.updateRoleChartYAxis();
  }

  generateStatusChartData(users: User[]) {
    const statusCount = users.reduce((acc, user) => {
      const status = this.formatState(user.state);
      if (!acc[status]) {
        acc[status] = 0;
      }
      acc[status]++;
      return acc;
    }, {} as Record<string, number>);
    
    this.statusChartData = Object.entries(statusCount)
      .map(([label, value]) => ({ label, value }))
      .sort((a, b) => b.value - a.value);
    
    this.updateStatusChartYAxis();
  }

  generateGenderChartData(users: User[]) {
    const genderCount = users.reduce((acc, user) => {
      const gender = user.sex === 0 ? 'Male' : 'Female';
      if (!acc[gender]) {
        acc[gender] = 0;
      }
      acc[gender]++;
      return acc;
    }, {} as Record<string, number>);
    
    this.genderChartData = Object.entries(genderCount)
      .map(([label, value]) => ({ label, value }))
      .sort((a, b) => b.value - a.value);
    
    this.updateGenderChartYAxis();
  }

  generateAgeGroupChartData(users: User[]) {
    const ageGroups = {
      '15-18': 0,
      '18-26': 0,
      '26-30': 0,
      '30-40': 0,
      '40-50': 0,
      '50-65': 0,
      '65+': 0
    };
    
    users.forEach(user => {
      const age = this.calculateAge(user.birthDate);
      
      if (age >= 15 && age <= 18) ageGroups['15-18']++;
      else if (age > 18 && age <= 26) ageGroups['18-26']++;
      else if (age > 26 && age <= 30) ageGroups['26-30']++;
      else if (age > 30 && age <= 40) ageGroups['30-40']++;
      else if (age > 40 && age <= 50) ageGroups['40-50']++;
      else if (age > 50 && age <= 65) ageGroups['50-65']++;
      else if (age > 65) ageGroups['65+']++;
    });
    
    this.ageGroupChartData = Object.entries(ageGroups)
      .filter(([_, value]) => value > 0)
      .map(([label, value]) => ({ label, value }));
    
    this.updateAgeGroupChartYAxis();
  }

  generateRegistrationChartData(users: User[]) {
    const monthNames = [
      'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
      'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
    ];
    
    const registrationByMonth = users.reduce((acc, user) => {
      const date = new Date(user.createdAt);
      const monthKey = `${monthNames[date.getMonth()]} ${date.getFullYear()}`;
      
      if (!acc[monthKey]) {
        acc[monthKey] = 0;
      }
      
      acc[monthKey]++;
      return acc;
    }, {} as Record<string, number>);
    
    const sortedEntries = Object.entries(registrationByMonth)
      .map(([label, value]) => {
        const [month, yearStr] = label.split(' ');
        const monthIndex = monthNames.indexOf(month);
        const year = parseInt(yearStr);
        return { label, value, sortKey: year * 100 + monthIndex };
      })
      .sort((a, b) => a.sortKey - b.sortKey)
      .map(({ label, value }) => ({ label, value }));
    
    this.registrationChartData = sortedEntries;
    this.updateRegistrationChartYAxis();
  }

  updateRoleChartYAxis() {
    this.updateCustomYAxis(this.roleChartData.map(item => item.value), this.roleChartYAxis);
  }
  
  updateStatusChartYAxis() {
    this.updateCustomYAxis(this.statusChartData.map(item => item.value), this.statusChartYAxis);
  }
  
  updateGenderChartYAxis() {
    this.updateCustomYAxis(this.genderChartData.map(item => item.value), this.genderChartYAxis);
  }
  
  updateAgeGroupChartYAxis() {
    this.updateCustomYAxis(this.ageGroupChartData.map(item => item.value), this.ageGroupChartYAxis);
  }
  
  updateRegistrationChartYAxis() {
    this.updateCustomYAxis(this.registrationChartData.map(item => item.value), this.registrationChartYAxis);
  }
  
  updateCustomYAxis(values: number[], targetAxis: number[]) {
    if (values.length === 0) {
      targetAxis.length = 0;
      targetAxis.push(0);
      return;
    }

    const maxValue = Math.max(...values);
    const steps = 5;

    const roundedMax = this.roundToNiceNumber(maxValue);
    const step = roundedMax / steps;

    targetAxis.length = 0;
    for (let i = 0; i <= steps; i++) {
      targetAxis.push(Math.round(roundedMax - i * step));
    }
  }

  resetUserFilters() {
    this.userStateFilter = 'all';
    this.userSexFilter = 'all';
    this.userRoleFilter = 'all';
    this.userAgeGroupFilter = 'all';
    this.userRegistrationTimeFilter = 'all';
    this.userLastUpdatedTimeFilter = 'all';
    
    this.resetFilterSelectElements();
    
    this.processUserData();
  }
  
  resetFilterSelectElements() {
    const selects = [
      'userRoleSelect',
      'userStateSelect', 
      'userSexSelect',
      'userAgeGroupSelect',
      'userRegistrationTimeSelect',
      'userLastUpdatedTimeSelect'
    ];
    
    selects.forEach(id => {
      const selectElement = document.getElementById(id) as HTMLSelectElement;
      if (selectElement) {
        selectElement.value = 'all';
      }
    });
  }
}
