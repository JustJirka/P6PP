import { Component, OnInit, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart } from 'chart.js/auto';
import { HttpClient } from '@angular/common/http';
import { catchError, finalize, of, tap } from 'rxjs';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';

interface DataItem {
  date: string;
  value: number;
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
}

enum BookingStatus {
  Confirmed = 0,
  Pending = 1,
  Cancelled = 2
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

const USERS_DATA: DataItem[] = [];

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, NavigationComponent, FooterComponent],
  templateUrl: './analytics.component.html',
  styleUrl: './analytics.component.scss',
})
export class AnalyticsComponent implements OnInit, AfterViewInit {
  Math = Math;
  
  activeTab: 'overview' | 'finances' | 'reservations' | 'users' = 'overview';
  viewMode: 'chart' | 'table' = 'chart';
  chartInstance: Chart | null = null;
  revenueChartInstance: Chart | null = null;
  
  financesData = FINANCES_DATA;
  reservationsData = RESERVATIONS_DATA;
  usersData = USERS_DATA;
  usersTableData: UserData[] = [];
  
  userChartType: UserChartType = 'registration';
  
  userRoleFilter = 'all';
  userStateFilter = 'all';
  userSexFilter = 'all';
  userAgeGroupFilter = 'all';
  userRegistrationTimeFilter = 'all';
  userBmiFilter = 'all';

  bookingStatusFilter = 'all';
  bookingDateFilter = 'all';
  bookingServiceFilter = 'all';
  bookingUserIdFilter = '';
  bookingDateRangeStart: string | null = null;
  bookingDateRangeEnd: string | null = null;
  uniqueServiceIds: number[] = [];
  uniqueUserIds: number[] = [];

  totalRevenue = this.getTotalFinanceValue();
  monthlyRevenue = this.getLatestFinanceValue();
  totalBookings = this.getTotalReservationsValue();
  monthlyBookings = this.getLatestReservationsValue();
  monthlyUsers = 0;
  
  isLoadingReservations = false;
  reservationsError: string | null = null;
  rawBookings: BookingDto[] = []; // raw booking data from API
  filteredBookings: BookingDto[] = []; // filtered booking data for display
  selectedBooking: BookingDto | null = null;
  showDetailedReservations = false;
  
  private bookingsApiUrl = 'http://localhost:8006/api/Bookings';
  private usersApiUrl = 'http://localhost:8006/api/Users';
  
  private currentDate = new Date();

  userAgeFrom: number = 0;
  userAgeTo: number = 0;
  userRegistrationFrom: string = '';  
  userRegistrationTo: string = '';

  // Přidám stav a typy pro daily analytics
  dailyAnalyticsType: 'revenue' | 'reservations' | 'newUsers' = 'revenue';
  private dailyAnalyticsTypes: Array<'revenue' | 'reservations' | 'newUsers'> = ['revenue', 'reservations', 'newUsers'];

  constructor(private http: HttpClient) {
    this.totalRevenue = this.getTotalFinanceValue();
    this.monthlyRevenue = this.getLatestFinanceValue();
    
    this.totalBookings = 0;
    this.monthlyBookings = 0;
  }

  ngOnInit() {
    this.fetchReservations();
    this.fetchUsers();
    this.initializeFilterOptions();

    // Simulované načítání dat
    this.usersTableData = this.getSampleUserData();
    
    this.initializeUserFilters();
  }

  ngAfterViewInit() {
    if (this.activeTab === 'overview') {
      this.dailyAnalyticsType = 'revenue';
      this.initializeDailyAnalyticsChart();
      this.updateCircularCharts();
    } else {
      this.renderChart();
    }
  }

  updateDashboardMetrics() {
    // Aktualizace metrik na analytics
    this.totalRevenue = this.getTotalFinanceValue();
    this.monthlyRevenue = this.getLatestFinanceValue();
    
    this.totalBookings = 0;
    this.monthlyBookings = 0;
    
    if (this.rawBookings.length > 0) {
      this.totalBookings = this.rawBookings.length;
      this.monthlyBookings = this.rawBookings.filter(b => this.isRecentReservation(b.bookingDate)).length;
    }
    
    this.monthlyUsers = this.getMonthlyUsers();
  }

  getMonthlyRevenueChange(): { value: number; isIncrease: boolean } {
    // Check if we have enough data to compare
    if (this.financesData.length < 2) return { value: 0, isIncrease: true };
    
    const currentMonthIndex = new Date().getMonth();
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const currentMonth = months[currentMonthIndex];
    const prevMonthIndex = (currentMonthIndex - 1 + 12) % 12; // Handle January correctly
    const prevMonth = months[prevMonthIndex];
    
    const currentMonthData = this.financesData.find(item => item.date === currentMonth);
    const prevMonthData = this.financesData.find(item => item.date === prevMonth);
    
    if (!currentMonthData || !prevMonthData || prevMonthData.value === 0) {
      return { value: 0, isIncrease: true };
    }
    
    const change = ((currentMonthData.value - prevMonthData.value) / prevMonthData.value) * 100;
    return {
      value: Math.abs(Math.round(change)),
      isIncrease: change >= 0
    };
  }

  getTotalRevenueChange(): { value: number; isIncrease: boolean } {
    // Calculate year-over-year change if possible, otherwise compare to previous month
    const currentYear = new Date().getFullYear();
    const lastYear = currentYear - 1;
    
    if (this.financesData.length < 2) return { value: 0, isIncrease: true };
    
    const firstQuarterData = this.financesData.slice(0, Math.min(3, this.financesData.length));
    const firstQuarterAvg = firstQuarterData.reduce((sum, item) => sum + item.value, 0) / firstQuarterData.length;
    
    const currentTotal = this.getTotalFinanceValue();
    const averagePerMonth = currentTotal / this.financesData.length;
    
    if (firstQuarterAvg === 0) return { value: 0, isIncrease: true };
    
    const change = ((averagePerMonth - firstQuarterAvg) / firstQuarterAvg) * 100;
    return {
      value: Math.abs(Math.round(change)),
      isIncrease: change >= 0
    };
  }

  getMonthlyBookingsChange(): { value: number; isIncrease: boolean } {
    const currentMonthIndex = new Date().getMonth();
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const currentMonth = months[currentMonthIndex];
    const prevMonthIndex = (currentMonthIndex - 1 + 12) % 12; // Handle January correctly
    const prevMonth = months[prevMonthIndex];
    
    const currentMonthData = this.reservationsData.find(item => item.date === currentMonth);
    const prevMonthData = this.reservationsData.find(item => item.date === prevMonth);
    
    // Handle cases where data might be missing
    if (!currentMonthData) {
      return { value: 0, isIncrease: true };
    }
    
    if (!prevMonthData || prevMonthData.value === 0) {
      return { value: 100, isIncrease: true };
    }
    
    const change = ((currentMonthData.value - prevMonthData.value) / prevMonthData.value) * 100;
    return {
      value: Math.abs(Math.round(change)),
      isIncrease: change >= 0
    };
  }

  getMonthlyUsersChange(): { value: number; isIncrease: boolean } {
    const currentMonthIndex = new Date().getMonth();
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const currentMonth = months[currentMonthIndex];
    const prevMonthIndex = (currentMonthIndex - 1 + 12) % 12; // Handle January correctly
    const prevMonth = months[prevMonthIndex];
    
    const currentMonthUsers = this.getMonthlyUsers(currentMonth);
    const prevMonthUsers = this.getMonthlyUsers(prevMonth);
    
    if (prevMonthUsers === 0) {
      return { value: 100, isIncrease: true };
    }
    
    const change = ((currentMonthUsers - prevMonthUsers) / prevMonthUsers) * 100;
    return {
      value: Math.abs(Math.round(change)),
      isIncrease: change >= 0
    };
  }

  getMonthlyUsers(month?: string): number {
    if (!month) {
      month = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'][new Date().getMonth()];
    }
    
    return this.usersTableData.filter(user => {
      const registrationMonth = this.getMonthName(user.createdAt);
      return registrationMonth === month;
    }).length;
  }

  fetchReservations() {
    this.isLoadingReservations = true;
    this.reservationsError = null;
    
    this.http.get<any[]>(this.bookingsApiUrl).pipe(
      tap(response => console.log('Raw reservation response:', response)),
      catchError(error => {
        console.error('Error fetching reservations data:', error);
        this.reservationsError = 'Failed to load reservation data. Using sample data.';
        return of([] as BookingDto[]);
      }),
      finalize(() => {
        this.isLoadingReservations = false;
      })
    ).subscribe(reservations => {
      if (reservations && reservations.length > 0) {
        // Normalize the data to ensure all required fields exist
        this.rawBookings = reservations.map(reservation => {
          let statusValue = reservation.status;
          if (statusValue === undefined && reservation.bookingStatus !== undefined) {
            statusValue = reservation.bookingStatus;
          }
          
          let statusText = statusValue;
          if (typeof statusValue === 'number') {
            switch(statusValue) {
              case BookingStatus.Confirmed:
                statusText = 'Confirmed';
                break;
              case BookingStatus.Pending:
                statusText = 'Pending';
                break;
              case BookingStatus.Cancelled:
                statusText = 'Cancelled';
                break;
              default:
                statusText = 'Unknown';
            }
          }
          
          return {
            id: reservation.id || 0,
            userId: reservation.userId || 0,
            serviceId: reservation.serviceId || 0,
            bookingDate: reservation.bookingDate || '',
            status: statusText || 'Unknown'
          };
        });
        
        console.log('Normalized reservations:', this.rawBookings);
        this.filteredBookings = [...this.rawBookings];
        this.initializeFilterOptions();
        this.processReservationsData(this.filteredBookings);
        this.updateDashboardMetrics();
      } else {
        console.log('No reservations found, using mock data');
        this.filteredBookings = [];
      }
    });
  }
  
  initializeFilterOptions() {
    this.uniqueServiceIds = Array.from(new Set(this.rawBookings.map(booking => booking.serviceId)))
      .filter(id => id !== undefined && id !== null)
      .sort((a, b) => a - b);
      
    this.uniqueUserIds = Array.from(new Set(this.rawBookings.map(booking => booking.userId)))
      .filter(id => id !== undefined && id !== null)
      .sort((a, b) => a - b);
  }

  processReservationsData(bookings: BookingDto[]) {
    const bookingsByMonth: {[key: string]: number} = {};
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    
    months.forEach(month => {
      bookingsByMonth[month] = 0;
    });
    
    bookings.forEach(booking => {
      if (booking.bookingDate) {
        const date = new Date(booking.bookingDate);
        const month = months[date.getMonth()];
        bookingsByMonth[month]++;
      }
    });
  
    const chartData = Object.keys(bookingsByMonth)
      .filter(month => months.includes(month))
      .map(month => ({
        date: month,
        value: bookingsByMonth[month]
      }))
      .sort((a, b) => {
        const monthOrder = { 'Jan': 0, 'Feb': 1, 'Mar': 2, 'Apr': 3, 'May': 4, 
                           'Jun': 5, 'Jul': 6, 'Aug': 7, 'Sep': 8, 'Oct': 9, 
                           'Nov': 10, 'Dec': 11 };
        return monthOrder[a.date as keyof typeof monthOrder] - monthOrder[b.date as keyof typeof monthOrder];
      });
    
    this.reservationsData = chartData;
    
    this.totalBookings = this.getTotalReservationsValue();
    this.monthlyBookings = this.getLatestReservationsValue();
    
    if (this.activeTab === 'reservations' && this.viewMode === 'chart') {
      setTimeout(() => this.renderChart(), 100);
    }
  }

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

  // RADŠI JSEM UDĚLAL I CREATE, KDYBY TO K NĚČEMU BYLO (KLIDNĚ SMAŽ)
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
  }*/
  
  deleteReservation(id: number) {
    this.http.delete<BookingDto>(`${this.bookingsApiUrl}/${id}`).pipe(
      tap(deletedBooking => {
        console.log('Deleted reservation:', deletedBooking);
        this.rawBookings = this.rawBookings.filter(booking => booking.id !== id);
        this.filteredBookings = this.filteredBookings.filter(booking => booking.id !== id);
        
        this.processReservationsData(this.filteredBookings);
      }),
      catchError(error => {
        console.error('Error deleting reservation:', error);
        alert('Failed to delete reservation');
        return of(null);
      })
    ).subscribe();
  }
  
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
    if (!status) return '';
    
    const statusLower = status.toLowerCase();
    
    if (statusLower === 'confirmed') {
      return 'success';
    }
    
    if (statusLower === 'pending') {
      return 'warning';
    }
    
    if (statusLower === 'cancelled') {
      return 'danger';
    }
    
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

  onBookingStatusFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.bookingStatusFilter = select.value;
    this.applyReservationFilters();
  }
  
  onBookingServiceFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.bookingServiceFilter = select.value;
    this.applyReservationFilters();
  }
  
  onBookingDateFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.bookingDateFilter = select.value;
    
    if (this.bookingDateFilter !== 'custom') {
      this.bookingDateRangeStart = null;
      this.bookingDateRangeEnd = null;
    }
    
    this.applyReservationFilters();
  }
  
  onBookingUserIdFilterChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.bookingUserIdFilter = input.value;
    this.applyReservationFilters();
  }
  
  onDateRangeStartChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.bookingDateRangeStart = input.value;
    this.applyReservationFilters();
  }
  
  onDateRangeEndChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.bookingDateRangeEnd = input.value;
    this.applyReservationFilters();
  }

  resetReservationFilters() {
    this.bookingStatusFilter = 'all';
    this.bookingDateFilter = 'all';
    this.bookingServiceFilter = 'all';
    this.bookingUserIdFilter = '';
    this.bookingDateRangeStart = null;
    this.bookingDateRangeEnd = null;
    
    const selects = [
      'bookingStatusSelect',
      'bookingDateSelect',
      'bookingServiceSelect'
    ];
    
    selects.forEach(id => {
      const select = document.getElementById(id) as HTMLSelectElement;
      if (select) select.value = 'all';
    });
    
    const userIdInput = document.getElementById('bookingUserIdInput') as HTMLInputElement;
    if (userIdInput) userIdInput.value = '';
    
    this.filteredBookings = [...this.rawBookings];
    this.processReservationsData(this.filteredBookings);
    
    if (this.viewMode === 'chart') {
      setTimeout(() => this.renderChart(), 100);
    }
  }
  
  applyReservationFilters() {
    this.filteredBookings = this.rawBookings.filter(booking => {
      // Status filter
      if (this.bookingStatusFilter !== 'all') {
        const bookingStatus = booking.status ? booking.status.toLowerCase() : '';
        if (bookingStatus !== this.bookingStatusFilter.toLowerCase()) {
          return false;
        }
      }
      
      // Service ID filter
      if (this.bookingServiceFilter !== 'all') {
        if (booking.serviceId !== parseInt(this.bookingServiceFilter)) {
          return false;
        }
      }
      
      // User ID filter
      if (this.bookingUserIdFilter && this.bookingUserIdFilter.trim() !== '') {
        if (booking.userId !== parseInt(this.bookingUserIdFilter)) {
          return false;
        }
      }
      
      // Date filter
      if (!booking.bookingDate) return true;
      
      const bookingDate = new Date(booking.bookingDate);
      const now = new Date();
      
      if (this.bookingDateFilter === 'custom') {
        if (this.bookingDateRangeStart) {
          const startDate = new Date(this.bookingDateRangeStart);
          if (bookingDate < startDate) {
            return false;
          }
        }
        
        if (this.bookingDateRangeEnd) {
          const endDate = new Date(this.bookingDateRangeEnd);
          endDate.setHours(23, 59, 59, 999); // End of day
          if (bookingDate > endDate) {
            return false;
          }
        }
      } else if (this.bookingDateFilter === 'today') {
        const today = new Date();
        if (bookingDate.getDate() !== today.getDate() ||
            bookingDate.getMonth() !== today.getMonth() ||
            bookingDate.getFullYear() !== today.getFullYear()) {
          return false;
        }
      } else if (this.bookingDateFilter === 'this_week') {
        const weekStart = new Date(now);
        weekStart.setDate(now.getDate() - now.getDay());
        weekStart.setHours(0, 0, 0, 0);
        
        const weekEnd = new Date(weekStart);
        weekEnd.setDate(weekStart.getDate() + 6);
        weekEnd.setHours(23, 59, 59, 999);
        
        if (bookingDate < weekStart || bookingDate > weekEnd) {
          return false;
        }
      } else if (this.bookingDateFilter === 'this_month') {
        if (bookingDate.getMonth() !== now.getMonth() ||
            bookingDate.getFullYear() !== now.getFullYear()) {
          return false;
        }
      } else if (this.bookingDateFilter === 'this_year') {
        if (bookingDate.getFullYear() !== now.getFullYear()) {
          return false;
        }
      }
      
      return true;
    });
    
    this.processReservationsData(this.filteredBookings);
    this.updateDashboardMetrics();
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

  initializeUserFilters() {
    const users = this.usersTableData;
    
    if (users.length > 0) {
      const ages = users.map(user => this.calculateAge(user.birthDate));
      this.userAgeFrom = Math.min(...ages);
      this.userAgeTo = Math.max(...ages);
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
      
      const age = this.calculateAge(user.birthDate);
      if (age < this.userAgeFrom || age > this.userAgeTo) {
        return false;
      }
      
      const registrationDate = new Date(user.createdAt);
      if (this.userRegistrationFrom && registrationDate < new Date(this.userRegistrationFrom)) {
        return false;
      }
      if (this.userRegistrationTo && registrationDate > new Date(this.userRegistrationTo)) {
        return false;
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

  onUserAgeFromChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.userAgeFrom = parseInt(input.value, 10);
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  onUserAgeToChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.userAgeTo = parseInt(input.value, 10);
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  onUserRegistrationFromChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.userRegistrationFrom = input.value;
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  onUserRegistrationToChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.userRegistrationTo = input.value;
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
    
    this.userAgeFrom = Math.min(...this.usersTableData.map(user => this.calculateAge(user.birthDate)));
    this.userAgeTo = Math.max(...this.usersTableData.map(user => this.calculateAge(user.birthDate)));
    this.userRegistrationFrom = '';
    this.userRegistrationTo = '';
    
    const ageFromInput = document.getElementById('userAgeFromInput') as HTMLInputElement;
    if (ageFromInput) ageFromInput.value = this.userAgeFrom.toString();
    
    const ageToInput = document.getElementById('userAgeToInput') as HTMLInputElement;
    if (ageToInput) ageToInput.value = this.userAgeTo.toString();
    
    const registrationFromInput = document.getElementById('userRegistrationFromInput') as HTMLInputElement;
    if (registrationFromInput) registrationFromInput.value = this.userRegistrationFrom;
    
    const registrationToInput = document.getElementById('userRegistrationToInput') as HTMLInputElement;
    if (registrationToInput) registrationToInput.value = this.userRegistrationTo;
    
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
    
    // If no data to display, don't try to create chart
    if (data.labels.length === 0) {
      console.log('No chart data available');
      return;
    }
    
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
      case 'users':
        return this.usersData;
      default:
        return this.financesData; 
    }
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
    
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const currentMonth = months[new Date().getMonth()];
    
    const currentMonthData = this.reservationsData.find(item => item.date === currentMonth);
    
    return currentMonthData ? currentMonthData.value : 0;
  }
  
  getTotalReservationsValue(): number {
    return this.reservationsData.reduce((sum, item) => sum + item.value, 0);
  }

  updateCircularCharts() {
    setTimeout(() => {
      const activeUsersCircle = document.querySelector('.users-overview .circle') as SVGPathElement;
      if (activeUsersCircle) {
        activeUsersCircle.setAttribute('stroke-dasharray', `${this.getActiveUsersPercentage()}, 100`);
      }
    }, 100);
  }

  navigateToTab(tab: 'overview' | 'finances' | 'reservations' | 'users') {
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

  setActiveTab(tab: 'overview' | 'finances' | 'reservations' | 'users') {
    this.activeTab = tab;
    
    if (tab === 'overview') {
      setTimeout(() => {
        this.dailyAnalyticsType = 'revenue';
        this.initializeDailyAnalyticsChart();
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
      this.resetReservationFilters();
    } else if (tab === 'users') {
      this.resetUserFilters();
    }
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
    return value.toString();
  }
  
  getValueDifference(current: number, previous: number): number {
    return Math.abs(current - previous);
  }

  // Přepínání grafů
  switchDailyAnalytics(direction: number) {
    const idx = this.dailyAnalyticsTypes.indexOf(this.dailyAnalyticsType);
    let newIdx = idx + direction;
    if (newIdx < 0) newIdx = this.dailyAnalyticsTypes.length - 1;
    if (newIdx >= this.dailyAnalyticsTypes.length) newIdx = 0;
    this.dailyAnalyticsType = this.dailyAnalyticsTypes[newIdx];
    setTimeout(() => this.initializeDailyAnalyticsChart(), 0);
  }

  // Titulek
  getDailyAnalyticsTitle(): string {
    switch (this.dailyAnalyticsType) {
      case 'revenue': return 'Daily Revenue';
      case 'reservations': return 'Daily Reservations';
      case 'newUsers': return 'Daily New Users';
      default: return '';
    }
  }
  // Legenda
  getDailyAnalyticsLegend(): string {
    switch (this.dailyAnalyticsType) {
      case 'revenue': return 'Revenue';
      case 'reservations': return 'Reservations';
      case 'newUsers': return 'New Users';
      default: return '';
    }
  }
  // Barva
  getDailyAnalyticsColor(): string {
    // Všechny grafy budou červené
    return '#ea2839';
  }

  // Získání denních dat pro aktuální měsíc
  getDailyRevenueData(): DataItem[] {
    const now = new Date();
    const days = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    // Pro každý den v měsíci spočítám sumu částek za všechny rezervace toho dne
    // Pokud není částka v datech, použiji simulovanou hodnotu
    const bookings = this.rawBookings.filter(b => {
      const d = new Date(b.bookingDate);
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear();
    });
    const byDay: { [key: string]: number } = {};
    for (let i = 1; i <= days; i++) byDay[i] = 0;
    bookings.forEach(b => {
      const d = new Date(b.bookingDate);
      const day = d.getDate();
      // Pokud existuje b.amount nebo b.price, použij ji, jinak simuluj
      const amount = (b as any).amount ?? (b as any).price ?? Math.floor(1000 + Math.random() * 1000);
      byDay[day] += amount;
    });
    return Array.from({ length: days }, (_, i) => ({ date: (i + 1).toString(), value: byDay[i + 1] }));
  }
  getDailyReservationsData(): DataItem[] {
    const now = new Date();
    const days = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    const bookings = this.rawBookings.filter(b => {
      const d = new Date(b.bookingDate);
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear();
    });
    const byDay: { [key: string]: number } = {};
    for (let i = 1; i <= days; i++) byDay[i] = 0;
    bookings.forEach(b => {
      const d = new Date(b.bookingDate);
      const day = d.getDate();
      byDay[day]++;
    });
    return Array.from({ length: days }, (_, i) => ({ date: (i + 1).toString(), value: byDay[i + 1] }));
  }
  getDailyNewUsersData(): DataItem[] {
    const now = new Date();
    const days = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    const users = this.usersTableData.filter(u => {
      const d = new Date(u.createdAt);
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear();
    });
    const byDay: { [key: string]: number } = {};
    for (let i = 1; i <= days; i++) byDay[i] = 0;
    users.forEach(u => {
      const d = new Date(u.createdAt);
      const day = d.getDate();
      byDay[day]++;
    });
    return Array.from({ length: days }, (_, i) => ({ date: (i + 1).toString(), value: byDay[i + 1] }));
  }

  // Inicializace grafu
  dailyAnalyticsChartInstance: Chart | null = null;
  initializeDailyAnalyticsChart() {
    setTimeout(() => {
      const canvas = document.getElementById('dailyAnalyticsChart') as HTMLCanvasElement;
      if (!canvas) return;
      if (this.dailyAnalyticsChartInstance) {
        this.dailyAnalyticsChartInstance.destroy();
      }
      let data: DataItem[] = [];
      switch (this.dailyAnalyticsType) {
        case 'revenue':
          data = this.getDailyRevenueData();
          break;
        case 'reservations':
          data = this.getDailyReservationsData();
          break;
        case 'newUsers':
          data = this.getDailyNewUsersData();
          break;
      }
      this.dailyAnalyticsChartInstance = new Chart(canvas, {
        type: 'line',
        data: {
          labels: data.map(d => d.date),
          datasets: [{
            label: this.getDailyAnalyticsLegend(),
            data: data.map(d => d.value),
            backgroundColor: this.getDailyAnalyticsColor() + '55',
            borderColor: this.getDailyAnalyticsColor(),
            borderWidth: 2,
            tension: 0.3,
            pointBackgroundColor: this.getDailyAnalyticsColor(),
            pointRadius: 3,
            fill: true
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: {
            y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.05)' } },
            x: { grid: { display: false }, title: { display: true, text: 'Day' } }
          }
        }
      });
    }, 100);
  }
}