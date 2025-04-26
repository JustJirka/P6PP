import { Component, OnInit, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart } from 'chart.js/auto';
import { HttpClient } from '@angular/common/http';
import { catchError, finalize, of, tap } from 'rxjs';

interface DataItem {
  date: string;
  value: number;
}

interface RoomData {
  roomId: string;
  roomName: string;
  occupancy: DataItem[];
}

interface UserData {
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

interface BookingDto {
  id: number;
  userId: number;
  serviceId: number;
  bookingDate: string;
  status: string;
  createdAt?: string; // optional
}

type BmiCategory = 'Underweight' | 'Normal' | 'Overweight' | 'Obese';

type UserChartType = 'registration' | 'role' | 'status' | 'gender' | 'bmi'| 'ageGroup' ;

const FINANCES_DATA: DataItem[] = [
  { date: 'Jan', value: 1250 },
  { date: 'Feb', value: 1960 },
  { date: 'Mar', value: 1580 },
  { date: 'Apr', value: 2130 },
  { date: 'May', value: 2470 },
];

const RESERVATIONS_DATA: DataItem[] = [
  { date: 'Jan', value: 156 },
  { date: 'Feb', value: 189 },
  { date: 'Mar', value: 172 },
  { date: 'Apr', value: 210 },
  { date: 'May', value: 243 },
];

const ROOMS_DATA: RoomData[] = [
  {
    roomId: 'room1',
    roomName: 'Fitness Studio A',
    occupancy: [
      { date: 'Jan', value: 65 },
      { date: 'Feb', value: 78 },
      { date: 'Mar', value: 72 },
      { date: 'Apr', value: 85 },
      { date: 'May', value: 90 },
    ],
  },
  {
    roomId: 'room2',
    roomName: 'Yoga Studio',
    occupancy: [
      { date: 'Jan', value: 45 },
      { date: 'Feb', value: 52 },
      { date: 'Mar', value: 60 },
      { date: 'Apr', value: 75 },
      { date: 'May', value: 82 },
    ],
  },
  {
    roomId: 'room3',
    roomName: 'Cardio Zone',
    occupancy: [
      { date: 'Jan', value: 80 },
      { date: 'Feb', value: 85 },
      { date: 'Mar', value: 82 },
      { date: 'Apr', value: 90 },
      { date: 'May', value: 95 },
    ],
  },
];

const USERS_DATA: DataItem[] = [];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit, AfterViewInit {
  Math = Math;
  
  activeTab: 'overview' | 'finances' | 'reservations' | 'rooms' | 'users' = 'overview';
  viewMode: 'chart' | 'table' = 'chart';
  selectedRoom: string | null = null;
  chartInstance: Chart | null = null;
  revenueChartInstance: Chart | null = null;
  
  financesData = FINANCES_DATA;
  reservationsData = RESERVATIONS_DATA;
  roomsData = ROOMS_DATA;
  usersData = USERS_DATA;
  usersTableData: UserData[] = [];
  
  userChartType: UserChartType = 'registration';
  
  userRoleFilter = 'all';
  userStateFilter = 'all';
  userSexFilter = 'all';
  userAgeGroupFilter = 'all';
  userRegistrationTimeFilter = 'all';
  userBmiFilter = 'all';

  totalRevenue = this.getTotalFinanceValue();
  monthlyRevenue = this.getLatestFinanceValue();
  totalBookings = this.getTotalReservationsValue();
  monthlyBookings = this.getLatestReservationsValue();
  averageOccupancy = this.getAverageRoomOccupancyValue();
  highestOccupancyRoom = this.getHighestOccupancyRoom();
  
  isLoadingReservations = false;
  reservationsError: string | null = null;
  rawBookings: BookingDto[] = []; // raw booking data from API
  selectedBooking: BookingDto | null = null;
  showDetailedReservations = false;
  
  private bookingsApiUrl = 'http://localhost:8006/api/Bookings';
  private usersApiUrl = 'http://localhost:8006/api/Users';
  
  private currentDate = new Date();

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchUsers();
    this.fetchReservations();
  }

  ngAfterViewInit() {
    this.initializeChart();
    this.initializeRevenueChart();    
    this.updateCircularCharts();
  }
  
  fetchReservations() {
    this.isLoadingReservations = true;
    this.reservationsError = null;
    
    this.http.get<BookingDto[]>(this.bookingsApiUrl).pipe(
      catchError(error => {
        console.error('Error fetching bookings data:', error);
        this.reservationsError = 'Failed to load reservation data. Using sample data.';
        return of([] as BookingDto[]);
      }),
      finalize(() => {
        this.isLoadingReservations = false;
      })
    ).subscribe(bookings => {
      if (bookings && bookings.length > 0) {
        this.rawBookings = bookings;
        
        this.processReservationsData(bookings);
      } else {
        console.log('No reservations found, using mock data');
      }
    });
  }
  
  processReservationsData(bookings: BookingDto[]) {
    const bookingsByMonth: {[key: string]: number} = {};
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    
    bookings.forEach(booking => {
      const dateStr = booking.createdAt || booking.bookingDate;
      const date = new Date(dateStr);
      const month = months[date.getMonth()];
      
      if (bookingsByMonth[month]) {
        bookingsByMonth[month]++;
      } else {
        bookingsByMonth[month] = 1;
      }
    });
    
    const chartData = Object.keys(bookingsByMonth).map(month => ({
      date: month,
      value: bookingsByMonth[month]
    }));
    
    if (chartData.length > 0) {
      this.reservationsData = chartData.sort((a, b) => {
        const monthOrder = { 'Jan': 0, 'Feb': 1, 'Mar': 2, 'Apr': 3, 'May': 4, 
                             'Jun': 5, 'Jul': 6, 'Aug': 7, 'Sep': 8, 'Oct': 9, 
                             'Nov': 10, 'Dec': 11 };
        return monthOrder[a.date as keyof typeof monthOrder] - monthOrder[b.date as keyof typeof monthOrder];
      });
      
      this.totalBookings = this.getTotalReservationsValue();
      this.monthlyBookings = this.getLatestReservationsValue();
      
      if (this.activeTab === 'reservations' && this.viewMode === 'chart') {
        setTimeout(() => this.renderChart(), 100);
      }
    }
  }
  
  // RADŠI JSEM UDĚLAL I CREATE A DELETE, KDYBY TO K NĚČEMU BYLO (KLIDNĚ SMAŽ)
  /*createReservation(booking: BookingDto) {
    this.http.post<BookingDto>(this.bookingsApiUrl, booking).pipe(
      tap(newBooking => {
        console.log('Created new reservation:', newBooking);
        this.rawBookings.push(newBooking);
        this.fetchReservations();
      }),
      catchError(error => {
        console.error('Error creating reservation:', error);
        alert('Failed to create reservation');
        return of(null);
      })
    ).subscribe();
  }
  
  deleteReservation(id: number) {
    this.http.delete<BookingDto>(`${this.bookingsApiUrl}/${id}`).pipe(
      tap(deletedBooking => {
        console.log('Deleted reservation:', deletedBooking);
        this.rawBookings = this.rawBookings.filter(booking => booking.id !== id);
        this.fetchReservations();
      }),
      catchError(error => {
        console.error('Error deleting reservation:', error);
        alert('Failed to delete reservation');
        return of(null);
      })
    ).subscribe();
  }*/
  
  // MŮŽEME VYUŽÍT FUNKCI PRO DATABASE SYNC - V HTML JE PAK TLAČÍTKO
  /*triggerDatabaseSync() {
    this.http.get<any>(`${this.bookingsApiUrl}/triggerSync`).pipe(
      tap(result => {
        console.log('Database sync triggered successfully:', result);
        alert('Database sync triggered successfully');
        this.fetchReservations();
      }),
      catchError(error => {
        console.error('Error triggering database sync:', error);
        alert('Error triggering database sync');
        return of(null);
      })
    ).subscribe();
  }*/
  
  toggleDetailedReservations() {
    this.showDetailedReservations = !this.showDetailedReservations;
    this.selectedBooking = null;
  }
  
  viewReservationDetails(booking: BookingDto) {
    this.selectedBooking = booking;
  }
  
  closeReservationDetails() {
    this.selectedBooking = null;
  }
  
  getReservationStatusClass(status: string): string {
    status = status.toLowerCase();
    if (status === 'confirmed' || status === 'completed') return 'success';
    if (status === 'pending') return 'warning';
    if (status === 'cancelled') return 'danger';
    return '';
  }
  
  formatReservationDate(dateString: string): string {
    const date = new Date(dateString);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}`;
  }
  
  isRecentReservation(dateString: string): boolean {
    const date = new Date(dateString);
    const thirtyDaysAgo = new Date(this.currentDate);
    thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
    return date >= thirtyDaysAgo;
  }
  
  getMonthName(dateString: string): string {
    const date = new Date(dateString);
    return ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'][date.getMonth()];
  }

  fetchUsers() {
    this.http.get<UserData[]>(this.usersApiUrl).subscribe(
      (data) => {
        this.usersTableData = data;
        this.generateUsersChartData();
      },
      (error) => {
        console.error('Error fetching users data:', error);
        this.usersTableData = this.getSampleUserData();
        this.generateUsersChartData();
      }
    );
  }

  getSampleUserData(): UserData[] {
    return [
      {
        id: 1,
        roleId: 1,
        state: 'active',
        sex: 0,
        weight: 80,
        height: 180,
        birthDate: '2002-01-15T00:00:00',
        createdAt: '2022-05-10T14:30:00',
        lastUpdated: '2022-05-10T14:30:00'
      },
      {
        id: 2,
        roleId: 0,
        state: 'active',
        sex: 1,
        weight: 65,
        height: 165,
        birthDate: '1998-08-22T00:00:00',
        createdAt: '2022-04-15T10:20:00',
        lastUpdated: '2023-01-12T09:45:00'
      },
      {
        id: 3,
        roleId: 0,
        state: 'inactive',
        sex: 0,
        weight: 95,
        height: 190,
        birthDate: '1985-03-10T00:00:00',
        createdAt: '2021-11-05T16:40:00',
        lastUpdated: '2022-12-20T11:30:00'
      },
      {
        id: 4,
        roleId: 1,
        state: 'active',
        sex: 1,
        weight: 58,
        height: 160,
        birthDate: '1992-06-30T00:00:00',
        createdAt: '2023-01-20T08:15:00',
        lastUpdated: '2023-02-28T14:20:00'
      },
      {
        id: 5,
        roleId: 0,
        state: 'active',
        sex: 0,
        weight: 88,
        height: 185,
        birthDate: '1990-11-12T00:00:00',
        createdAt: '2022-07-08T11:10:00',
        lastUpdated: '2023-03-15T10:05:00'
      }
    ];
  }

  generateUsersChartData() {
    const filteredUsers = this.getFilteredUsers();
    
    switch (this.userChartType) {
      case 'registration':
        this.generateRegistrationChartData(filteredUsers);
        break;
      case 'role':
      case 'status':
      case 'gender':
      case 'ageGroup':
        this.generateDistributionChartData(filteredUsers, this.userChartType);
        break;
      case 'bmi':
        this.generateBmiChartData(filteredUsers);
        break;
    }
  }

  getFilteredUsers(): UserData[] {
    return this.usersTableData.filter(user => {
      if (this.userRoleFilter !== 'all' && user.roleId.toString() !== this.userRoleFilter) {
        return false;
      }
      
      if (this.userStateFilter !== 'all' && user.state !== this.userStateFilter) {
        return false;
      }
      
      if (this.userSexFilter !== 'all' && user.sex.toString() !== this.userSexFilter) {
        return false;
      }
      
      if (this.userAgeGroupFilter !== 'all') {
        const age = this.calculateAge(user.birthDate);
        const [minAge, maxAge] = this.userAgeGroupFilter.split('-');
        
        if (maxAge && (age < parseInt(minAge) || age > parseInt(maxAge))) {
          return false;
        } else if (!maxAge && age < parseInt(minAge)) {
          return false;
        }
      }
      
      if (this.userRegistrationTimeFilter !== 'all') {
        const registrationDate = new Date(user.createdAt);
        const now = new Date();
        
        if (this.userRegistrationTimeFilter === 'week' && 
            (now.getTime() - registrationDate.getTime() > 7 * 24 * 60 * 60 * 1000)) {
          return false;
        } else if (this.userRegistrationTimeFilter === 'month' && 
                  (now.getMonth() !== registrationDate.getMonth() || 
                   now.getFullYear() !== registrationDate.getFullYear())) {
          return false;
        } else if (this.userRegistrationTimeFilter === 'year' && 
                  now.getFullYear() !== registrationDate.getFullYear()) {
          return false;
        }
      }
      
      if (this.userBmiFilter !== 'all') {
        const bmi = this.calculateBMI(user.weight, user.height);
        const bmiCategory = this.getBMICategory(bmi);
        
        if (bmiCategory.toLowerCase() !== this.userBmiFilter.toLowerCase()) {
          return false;
        }
      }
      
      return true;
    });
  }

  generateRegistrationChartData(users: UserData[]) {
    const registrationsByMonth: {[key: string]: number} = {};
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    
    users.forEach(user => {
      const date = new Date(user.createdAt);
      const monthYear = `${months[date.getMonth()]} ${date.getFullYear()}`;
      
      if (registrationsByMonth[monthYear]) {
        registrationsByMonth[monthYear]++;
      } else {
        registrationsByMonth[monthYear] = 1;
      }
    });
    
    this.usersData = Object.keys(registrationsByMonth)
      .sort((a, b) => {
        const [aMonth, aYear] = a.split(' ');
        const [bMonth, bYear] = b.split(' ');
        return parseInt(aYear) - parseInt(bYear) || 
               months.indexOf(aMonth) - months.indexOf(bMonth);
      })
      .map(monthYear => ({
        date: monthYear,
        value: registrationsByMonth[monthYear]
      }));
  }

  generateDistributionChartData(users: UserData[], chartType: string) {
    let distributionData: {[key: string]: number} = {};
    
    switch (chartType) {
      case 'role':
        users.forEach(user => {
          const role = this.formatRole(user.roleId);
          distributionData[role] = (distributionData[role] || 0) + 1;
        });
        break;
        
      case 'status':
        users.forEach(user => {
          const status = this.formatState(user.state);
          distributionData[status] = (distributionData[status] || 0) + 1;
        });
        break;
        
      case 'gender':
        users.forEach(user => {
          const gender = this.formatSex(user.sex);
          distributionData[gender] = (distributionData[gender] || 0) + 1;
        });
        break;
        
      case 'ageGroup':
        const ageGroups = ['15-18', '18-26', '26-35', '35-45', '45-55', '55-65', '65+'];
        ageGroups.forEach(group => distributionData[group] = 0);
        
        users.forEach(user => {
          const age = this.calculateAge(user.birthDate);
          let ageGroup = '';
          
          if (age < 18) ageGroup = '15-18';
          else if (age < 26) ageGroup = '18-26';
          else if (age < 35) ageGroup = '26-35';
          else if (age < 45) ageGroup = '35-45';
          else if (age < 55) ageGroup = '45-55';
          else if (age < 65) ageGroup = '55-65';
          else ageGroup = '65+';
          
          distributionData[ageGroup]++;
        });
        break;
    }
    
    this.usersData = Object.keys(distributionData).map(key => ({
      date: key,
      value: distributionData[key]
    }));
  }

  onChartTypeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.userChartType = select.value as UserChartType;
    this.generateUsersChartData();
    this.renderChart();
  }

  onUserRoleFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.userRoleFilter = select.value;
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  onUserStateFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.userStateFilter = select.value;
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  onUserSexFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.userSexFilter = select.value;
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  onUserAgeGroupFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.userAgeGroupFilter = select.value;
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  onUserRegistrationTimeFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.userRegistrationTimeFilter = select.value;
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  onUserBmiFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.userBmiFilter = select.value;
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  resetUserFilters() {
    this.userRoleFilter = 'all';
    this.userStateFilter = 'all';
    this.userSexFilter = 'all';
    this.userAgeGroupFilter = 'all';
    this.userRegistrationTimeFilter = 'all';
    this.userBmiFilter = 'all';
    
    const selects = [
      'userRoleSelect',
      'userStateSelect',
      'userSexSelect',
      'userAgeGroupSelect',
      'userRegistrationTimeSelect',
      'userBmiSelect'
    ];
    
    selects.forEach(id => {
      const select = document.getElementById(id) as HTMLSelectElement;
      if (select) select.value = 'all';
    });
    
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  formatRole(roleId: number): string {
    return roleId === 1 ? 'Trainer' : 'User';
  }

  formatState(state: string): string {
    return state.charAt(0).toUpperCase() + state.slice(1);
  }

  formatSex(sex: number): string {
    return sex === 0 ? 'Male' : 'Female';
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    return `${months[date.getMonth()]} ${date.getDate()}, ${date.getFullYear()}`;
  }

  calculateAge(birthDateString: string): number {
    const birthDate = new Date(birthDateString);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    
    return age;
  }

  calculateBMI(weight: number, height: number): number {
    if (!weight || !height) return 0;
    const heightInMeters = height / 100;
    return weight / (heightInMeters * heightInMeters);
  }

  formatBMI(bmi: number): string {
    if (!bmi) return 'N/A';
    return bmi.toFixed(1);
  }

  getBMIClass(bmi: number): string {
    if (!bmi) return '';
    if (bmi < 18.5) return 'underweight';
    if (bmi < 25) return 'normal';
    if (bmi < 30) return 'overweight';
    return 'obese';
  }

  getTotalUsersCount(): number {
    return this.usersTableData.length;
  }

  getActiveUsersCount(): number {
    return this.usersTableData.filter(user => user.state === 'active').length;
  }

  getActiveUsersPercentage(): number {
    if (this.usersTableData.length === 0) return 0;
    return Math.round((this.getActiveUsersCount() / this.getTotalUsersCount()) * 100);
  }

  initializeChart() {
    setTimeout(() => {
      this.renderChart();
    }, 0);
  }

  initializeRevenueChart() {
    setTimeout(() => {
      const canvas = document.getElementById('revenueChart') as HTMLCanvasElement;
      if (!canvas) return;
      
      if (this.revenueChartInstance) {
        this.revenueChartInstance.destroy();
      }
      
      this.revenueChartInstance = new Chart(canvas, {
        type: 'line',
        data: {
          labels: this.financesData.map(item => item.date),
          datasets: [{
            label: 'Revenue',
            data: this.financesData.map(item => item.value),
            backgroundColor: 'rgba(234, 40, 56, 0.32)',
            borderColor: 'rgb(234, 40, 56)',
            borderWidth: 2,
            tension: 0.3,
            pointBackgroundColor: 'rgb(234, 40, 56)',
            pointRadius: 4,
            fill: true
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              display: false
            }
          },
          scales: {
            y: {
              beginAtZero: true,
              grid: {
                color: 'rgba(0, 0, 0, 0.05)'
              }
            },
            x: {
              grid: {
                display: false
              }
            }
          }
        }
      });
    }, 100);
  }

  renderChart() {
    const canvas = document.getElementById('dataChart') as HTMLCanvasElement;
    if (!canvas) return;
    
    if (this.chartInstance) {
      this.chartInstance.destroy();
    }
    
    const data = this.getChartData();
    let chartType: 'line' | 'bar' = 'line';
    let chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          backgroundColor: 'rgba(0, 0, 0, 0.7)',
          titleColor: 'white',
          bodyColor: 'white',
          padding: 10,
          cornerRadius: 4
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          grid: {
            color: 'rgba(0, 0, 0, 0.05)'
          },
          title: {
            display: true,
            text: this.getYAxisLabel()
          }
        },
        x: {
          grid: {
            display: false
          },
          title: {
            display: true,
            text: this.getXAxisLabel()
          }
        }
      }
    };
    
    if (this.activeTab === 'users' && this.userChartType !== 'registration') {
      chartType = 'bar';
    }
    
    let datasets = [{
      label: this.getValueLabel(),
      data: data.values,
      backgroundColor: 'rgba(234, 40, 56, 0.32)',
      borderColor: 'rgb(234, 40, 56)',
      borderWidth: 2,
      tension: 0.3,
      pointBackgroundColor: 'rgb(234, 40, 56)',
      pointRadius: 4,
      fill: chartType === 'line' ? true : false
    }];
    
    this.chartInstance = new Chart(canvas, {
      type: chartType,
      data: {
        labels: data.labels,
        datasets: datasets
      },
      options: chartOptions as any
    });
  }

  getChartData() {
    const data = this.getActiveData();
    return {
      labels: data.map(item => item.date),
      values: data.map(item => item.value)
    };
  }

  getActiveData(): DataItem[] {
    switch (this.activeTab) {
      case 'finances':
        return this.financesData;
      case 'reservations':
        return this.reservationsData;
      case 'rooms':
        if (this.selectedRoom) {
          const room = this.roomsData.find(r => r.roomId === this.selectedRoom);
          return room ? room.occupancy : [];
        }
        return this.getAverageRoomOccupancy();
      case 'users':
        return this.usersData;
      default:
        return this.financesData; 
    }
  }

  getAverageRoomOccupancy(): DataItem[] {
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May'];
    return months.map(month => {
      const occupancies = this.roomsData.flatMap(room => 
        room.occupancy.filter(item => item.date === month)
      );
      
      const avgValue = occupancies.length > 0
        ? occupancies.reduce((sum, item) => sum + item.value, 0) / occupancies.length
        : 0;
        
      return { date: month, value: Math.round(avgValue) };
    });
  }

  setActiveTab(tab: 'overview' | 'finances' | 'reservations' | 'rooms' | 'users') {
    this.activeTab = tab;
    if (tab !== 'rooms') {
      this.selectedRoom = null;
    }
    
    if (tab === 'overview') {
      setTimeout(() => {
        this.initializeRevenueChart();
        this.updateCircularCharts();
      }, 0);
    } else {
      setTimeout(() => {
        this.renderChart();
      }, 0);
    }
    
    // Reset detailed reservations view when switching tabs
    if (tab === 'reservations') {
      this.showDetailedReservations = false;
    }
  }

  selectRoom(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.selectedRoom = select.value || null;
    this.renderChart();
  }

  toggleView() {
    this.viewMode = this.viewMode === 'chart' ? 'table' : 'chart';
    if (this.viewMode === 'chart') {
      setTimeout(() => {
        this.renderChart();
      }, 0);
    }
  }

  getActiveTitle(): string {
    switch (this.activeTab) {
      case 'overview':
        return 'Dashboard Overview';
      case 'finances':
        return 'Financial Analytics';
      case 'reservations':
        return 'Reservation Statistics';
      case 'rooms':
        if (this.selectedRoom) {
          const room = this.roomsData.find(r => r.roomId === this.selectedRoom);
          return room ? `${room.roomName} Occupancy` : 'Room Occupancy';
        }
        return 'Average Room Occupancy';
      case 'users':
        return 'User Statistics';
      default:
        return 'Dashboard';
    }
  }

  getYAxisLabel(): string {
    switch (this.activeTab) {
      case 'finances':
        return 'Revenue ($)';
      case 'reservations':
        return 'Number of Reservations';
      case 'rooms':
        return 'Occupancy (%)';
      case 'users':
        switch (this.userChartType) {
          case 'registration':
            return 'Number of Registrations';
          case 'role':
          case 'status':
          case 'gender':
          case 'ageGroup':
            return 'Number of Users';
          case 'bmi':
            return 'Number of Users';
          default:
            return 'Count';
        }
      default:
        return 'Value';
    }
  }

  getXAxisLabel(): string {
    switch (this.activeTab) {
      case 'finances':
      case 'reservations':
      case 'rooms':
        return 'Month';
      case 'users':
        switch (this.userChartType) {
          case 'registration':
            return 'Registration Period';
          case 'role':
            return 'User Role';
          case 'status':
            return 'User Status';
          case 'gender':
            return 'Gender';
          case 'ageGroup':
            return 'Age Group (years)';
          case 'bmi':
            return 'BMI Category';
          default:
            return 'Category';
        }
      default:
        return 'Period';
    }
  }

  getValueLabel(): string {
    switch (this.activeTab) {
      case 'finances':
        return 'Revenue ($)';
      case 'reservations':
        return 'Number of Reservations';
      case 'rooms':
        return 'Occupancy (%)';
      case 'users':
        switch (this.userChartType) {
          case 'registration':
            return 'New Registrations';
          case 'role':
            return 'Users by Role';
          case 'status':
            return 'Users by Status';
          case 'gender':
            return 'Users by Gender';
          case 'ageGroup':
            return 'Users by Age Group';
          case 'bmi':
            return 'Users by BMI';
          default:
            return 'Number of Users';
        }
      default:
        return 'Value';
    }
  }

  formatValue(value: number): string {
    if (this.activeTab === 'finances') {
      return `$${value}`;
    }
    if (this.activeTab === 'rooms') {
      return `${value}%`;
    }
    return value.toString();
  }
  
  getValueDifference(current: number, previous: number): number {
    return Math.abs(current - previous);
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
    const allOccupancies = this.roomsData.flatMap(room => room.occupancy);
    if (allOccupancies.length === 0) return 0;
    
    const total = allOccupancies.reduce((sum, item) => sum + item.value, 0);
    return Math.round(total / allOccupancies.length);
  }

  getHighestOccupancyRoom(): { name: string; value: number } {
    if (this.roomsData.length === 0) {
      return { name: 'None', value: 0 };
    }
    
    const roomAverages = this.roomsData.map(room => {
      const total = room.occupancy.reduce((sum, item) => sum + item.value, 0);
      const average = Math.round(total / room.occupancy.length);
      return { 
        name: room.roomName, 
        value: average 
      };
    });
    
    return roomAverages.reduce((highest, current) => 
      (current.value > highest.value ? current : highest), 
      { name: 'None', value: 0 }
    );
  }

  updateCircularCharts() {
    setTimeout(() => {
      const activeUsersCircle = document.querySelector('.users-overview .circle') as SVGPathElement;
      if (activeUsersCircle) {
        activeUsersCircle.setAttribute('stroke-dasharray', `${this.getActiveUsersPercentage()}, 100`);
      }
      
      const occupancyCircle = document.querySelector('.occupancy-metric .circle') as SVGPathElement;
      if (occupancyCircle) {
        occupancyCircle.setAttribute('stroke-dasharray', `${this.averageOccupancy}, 100`);
      }
    }, 100);
  }

  navigateToTab(tab: 'overview' | 'finances' | 'reservations' | 'rooms' | 'users') {
    this.setActiveTab(tab);
  }

  getBMICategory(bmi: number): BmiCategory {
    if (bmi < 18.5) return 'Underweight';
    if (bmi < 25) return 'Normal';
    if (bmi < 30) return 'Overweight';
    return 'Obese';
  }

  generateBmiChartData(users: UserData[]) {
    const bmiCategories: BmiCategory[] = ['Underweight', 'Normal', 'Overweight', 'Obese'];
    const bmiData: {[key in BmiCategory]: number} = {
      'Underweight': 0,
      'Normal': 0,
      'Overweight': 0,
      'Obese': 0
    };
    
    users.forEach(user => {
      const bmi = this.calculateBMI(user.weight, user.height);
      if (bmi) {
        const category = this.getBMICategory(bmi);
        bmiData[category]++;
      }
    });
    
    this.usersData = bmiCategories.map(category => ({
      date: category,
      value: bmiData[category]
    }));
  }
}