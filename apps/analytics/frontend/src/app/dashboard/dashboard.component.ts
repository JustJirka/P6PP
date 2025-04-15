import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

// Definice typů dat
interface DataItem {
  date: string;
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
  sex: number; // 0 for men, 1 for women
  weight: number;
  height: number;
  birthDate: string;
  createdAt: string;
  lastUpdated: string;
}

// Typy pro časové rozsahy a věkové skupiny
type TimeRange = 'all' | 'week' | 'month' | 'year';
type AgeGroup = 'all' | '15-18' | '18-26' | '26-30' | '30-40' | '40-50' | '50-65' | '65+';
type UserRole = 'all' | 'user' | 'trainer';

// Data pro finance
const FINANCES_DATA: DataItem[] = [
  { date: '2023-01', value: 1250 },
  { date: '2023-02', value: 1960 },
  { date: '2023-03', value: 1580 },
  { date: '2023-04', value: 2130 },
  { date: '2023-05', value: 2470 },
];

// Data pro rezervace
const RESERVATIONS_DATA: DataItem[] = [
  { date: '2023-01', value: 156 },
  { date: '2023-02', value: 189 },
  { date: '2023-03', value: 172 },
  { date: '2023-04', value: 210 },
  { date: '2023-05', value: 243 },
];

// Data pro místnosti
const ROOMS_DATA: RoomData[] = [
  {
    roomId: 'room1',
    roomName: 'Fitness Studio A',
    occupancy: [
      { date: '2023-01', value: 65 }, // hodnota v procentech
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

// Funkce pro generování přehledových dat
const generateOverviewData = (): DataItem[] => {
  const result: DataItem[] = [];
  const months = ['2023-01', '2023-02', '2023-03', '2023-04', '2023-05'];

  months.forEach((month) => {
    // Suma financí za daný měsíc
    const financeValue =
      FINANCES_DATA.find((item) => item.date === month)?.value || 0;

    // Počet rezervací za daný měsíc
    const reservationsValue =
      RESERVATIONS_DATA.find((item) => item.date === month)?.value || 0;

    // Průměrná obsazenost místností za daný měsíc
    const roomOccupancies = ROOMS_DATA.flatMap((room) =>
      room.occupancy.filter((item) => item.date === month)
    );
    const avgOccupancy =
      roomOccupancies.length > 0
        ? roomOccupancies.reduce((sum, item) => sum + item.value, 0) /
          roomOccupancies.length
        : 0;

    // Celková hodnota pro přehled - můžete upravit podle vašich metrik
    // Zde používám vážený průměr - finance mají největší váhu
    const totalValue =
      financeValue * 0.6 +
      reservationsValue * 50 * 0.3 +
      avgOccupancy * 100 * 0.1;

    result.push({ date: month, value: Math.round(totalValue) });
  });

  return result;
};

// Generování dat pro overview
const OVERVIEW_DATA = generateOverviewData();

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  // Typy záložek
  activeTab:
    | 'overview'
    | 'finances'
    | 'reservations'
    | 'rooms'
    | 'users'
    | 'trainers' = 'overview';
  viewMode: 'chart' | 'table' = 'chart';
  selectedRoom: string | null = null;
  
  // User filters
  userStateFilter: 'all' | 'active' | 'inactive' = 'all';
  userSexFilter: 'all' | 'male' | 'female' = 'all';
  userRoleFilter: UserRole = 'all';
  userAgeGroupFilter: AgeGroup = 'all';
  userRegistrationTimeFilter: TimeRange = 'all';
  userLastUpdatedTimeFilter: TimeRange = 'all';

  // Data pro jednotlivé záložky
  overviewData = OVERVIEW_DATA;
  financesData = FINANCES_DATA;
  reservationsData = RESERVATIONS_DATA;
  roomsData = ROOMS_DATA;
  usersData: DataItem[] = [];
  usersTableData: User[] = [];
  trainersData: DataItem[] = [];
  
  // Data from API
  users: User[] = [];
  isUsersLoading = false;
  usersError: string | null = null;

  yAxisValues: number[] = [];

  constructor(private http: HttpClient) {}

  // Method to load users from API
  loadUsers() {
    this.isUsersLoading = true;
    this.usersError = null;
    
    this.http.get<User[]>('http://localhost:8006/api/Users', { withCredentials: false })
      .subscribe({
        next: (data) => {
          this.users = data;
          this.processUserData();
          this.isUsersLoading = false;
        },
        error: (error) => {
          console.error('Chyba při načítání uživatelů:', error);
          this.usersError = 'Failed to load data from API.';
          this.isUsersLoading = false;
        }
      });
  }

  // Method to process user data based on all filters
  processUserData() {
    if (!this.users || this.users.length === 0) {
      this.usersData = [];
      this.usersTableData = [];
      return;
    }

    // Apply all filters
    let filteredUsers = this.applyAllUserFilters(this.users);
    
    // Update table data
    this.usersTableData = filteredUsers;
    
    // Getting months from createdAt and grouping users by months for the chart view
    const usersByMonth = filteredUsers.reduce((acc, user) => {
      const date = new Date(user.createdAt);
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
      
      if (!acc[monthKey]) {
        acc[monthKey] = 0;
      }
      
      acc[monthKey]++;
      return acc;
    }, {} as Record<string, number>);
    
    // Convert to DataItem array and sort by date
    this.usersData = Object.entries(usersByMonth)
      .map(([date, value]) => ({ date, value }))
      .sort((a, b) => a.date.localeCompare(b.date));
    
    this.updateYAxisValues();
  }

  // Apply all user filters at once
  applyAllUserFilters(users: User[]): User[] {
    let filteredUsers = [...users];
    
    // Filter by role
    if (this.userRoleFilter !== 'all') {
      const roleIdToFilter = this.userRoleFilter === 'user' ? 0 : 1;
      filteredUsers = filteredUsers.filter(user => user.roleId === roleIdToFilter);
    }
    
    // Filter by state
    if (this.userStateFilter !== 'all') {
      const stateToFilter = this.userStateFilter === 'active' ? 'active' : 'inactive';
      filteredUsers = filteredUsers.filter(user => user.state === stateToFilter);
    }
    
    // Filter by sex
    if (this.userSexFilter !== 'all') {
      const sexToFilter = this.userSexFilter === 'male' ? 0 : 1;
      filteredUsers = filteredUsers.filter(user => user.sex === sexToFilter);
    }
    
    // Filter by age group
    if (this.userAgeGroupFilter !== 'all') {
      filteredUsers = filteredUsers.filter(user => this.isUserInAgeGroup(user, this.userAgeGroupFilter));
    }
    
    // Filter by registration time
    if (this.userRegistrationTimeFilter !== 'all') {
      filteredUsers = filteredUsers.filter(user => 
        this.isDateInTimeRange(new Date(user.createdAt), this.userRegistrationTimeFilter)
      );
    }
    
    // Filter by last updated time
    if (this.userLastUpdatedTimeFilter !== 'all') {
      filteredUsers = filteredUsers.filter(user => 
        this.isDateInTimeRange(new Date(user.lastUpdated), this.userLastUpdatedTimeFilter)
      );
    }
    
    return filteredUsers;
  }

  // Check if a date is within the specified time range
  isDateInTimeRange(date: Date, range: TimeRange): boolean {
    if (range === 'all') return true;
    
    const now = new Date();
    
    if (range === 'week') {
      // Within the last 7 days
      const oneWeekAgo = new Date();
      oneWeekAgo.setDate(now.getDate() - 7);
      return date >= oneWeekAgo;
    }
    
    if (range === 'month') {
      // Within the last 30 days
      const oneMonthAgo = new Date();
      oneMonthAgo.setDate(now.getDate() - 30);
      return date >= oneMonthAgo;
    }
    
    if (range === 'year') {
      // Within the last 365 days
      const oneYearAgo = new Date();
      oneYearAgo.setFullYear(now.getFullYear() - 1);
      return date >= oneYearAgo;
    }
    
    return false;
  }

  // Check if a user is in the specified age group
  isUserInAgeGroup(user: User, ageGroup: AgeGroup): boolean {
    if (ageGroup === 'all') return true;
    
    const birthDate = new Date(user.birthDate);
    const today = new Date();
    
    // Calculate age
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    
    // Check against age groups
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

  // Calculate BMI for a user
  calculateBMI(weight: number, height: number): number {
    // Height should be in meters, so convert from cm
    const heightInMeters = height / 100;
    // BMI formula: weight(kg) / height²(m)
    return weight / (heightInMeters * heightInMeters);
  }

  // Format BMI value with proper classification
  formatBMI(bmi: number): string {
    // Round to 1 decimal place
    const roundedBMI = Math.round(bmi * 10) / 10;
    
    // BMI classification
    let classification = '';
    if (bmi < 18.5) classification = '(Underweight)';
    else if (bmi < 25) classification = '(Normal)';
    else if (bmi < 30) classification = '(Overweight)';
    else classification = '(Obese)';
    
    return `${roundedBMI} ${classification}`;
  }

  // Get CSS class for BMI value
  getBMIClass(bmi: number): string {
    if (bmi < 18.5) return 'bmi-underweight';
    else if (bmi < 25) return 'bmi-normal';
    else if (bmi < 30) return 'bmi-overweight';
    else return 'bmi-obese';
  }

  // Calculate age from birth date
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

  // Filter handlers for user data
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
  
  // Event handler for filter changes in select elements
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

  // Format methods for display
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
        // Pokud není vybrána žádná místnost, vrátíme průměrnou obsazenost všech místností
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

  // Metoda pro výpočet průměrné obsazenosti místností
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
    // Reset selected room when changing tabs
    if (tab !== 'rooms') {
      this.selectedRoom = null;
    }
    // Reset filters when tab is users
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

    // Zaokrouhlíme maximum na "hezké" číslo pro lepší čitelnost
    const roundedMax = this.roundToNiceNumber(maxValue);
    const step = roundedMax / steps;

    this.yAxisValues = Array.from({ length: steps + 1 }, (_, i) => {
      // Zobrazíme hodnoty od shora dolů
      return Math.round(roundedMax - i * step);
    });
  }

  // Nová pomocná metoda pro zaokrouhlení na "hezké" číslo
  roundToNiceNumber(value: number): number {
    // Pro malé hodnoty použijeme přesnost na jednotky
    if (value < 10) return Math.ceil(value);

    // Pro hodnoty 10-100 zaokrouhlíme na desítky
    if (value < 100) return Math.ceil(value / 10) * 10;

    // Pro hodnoty 100-1000 zaokrouhlíme na stovky
    if (value < 1000) return Math.ceil(value / 100) * 100;

    // Pro hodnoty 1000-10000 zaokrouhlíme na tisíce
    if (value < 10000) return Math.ceil(value / 1000) * 1000;

    // Pro větší hodnoty zaokrouhlíme na desetitisíce
    return Math.ceil(value / 10000) * 10000;
  }

  getBarHeight(value: number): number {
    const maxAxisValue = this.yAxisValues[0]; // První hodnota je nejvyšší
    if (maxAxisValue === 0) return 0;

    // Vypočítáme výšku jako procento z celkové dostupné výšky grafu (250px)
    // Použijeme matematickou proporci: barHeight / chartHeight = value / maxAxisValue
    return (value / maxAxisValue) * 250;
  }

  // Get the latest (most recent) finance value
  getLatestFinanceValue(): number {
    if (this.financesData.length === 0) return 0;
    return this.financesData[this.financesData.length - 1].value;
  }

  // Calculate total finance value
  getTotalFinanceValue(): number {
    return this.financesData.reduce((sum, item) => sum + item.value, 0);
  }

  // Get the latest reservations value
  getLatestReservationsValue(): number {
    if (this.reservationsData.length === 0) return 0;
    return this.reservationsData[this.reservationsData.length - 1].value;
  }

  // Calculate total reservations
  getTotalReservationsValue(): number {
    return this.reservationsData.reduce((sum, item) => sum + item.value, 0);
  }

  // Calculate average room occupancy across all rooms and months
  getAverageRoomOccupancyValue(): number {
    const allOccupancies = this.roomsData.flatMap((room) => room.occupancy);
    if (allOccupancies.length === 0) return 0;

    const total = allOccupancies.reduce((sum, item) => sum + item.value, 0);
    return Math.round(total / allOccupancies.length);
  }

  // Find the room with highest average occupancy
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
  
  // Get total number of users
  getTotalUsersCount(): number {
    return this.users.length;
  }
  
  // Get number of active users
  getActiveUsersCount(): number {
    return this.users.filter(user => user.state === 'active').length;
  }
}
