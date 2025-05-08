import { Component, OnInit, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart } from 'chart.js/auto';
import { HttpClient } from '@angular/common/http';
import { catchError, finalize, of, tap } from 'rxjs';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { environment } from '../../../enviroments/enviroment';

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

// Aktualizovaný model pro rezervace, služby a místnosti podle poskytnutého JSONu
interface Room {
  name: string;
  capacity: number;
}

interface Service {
  trainerId: number;
  serviceName: string;
  start: string;
  end: string;
  roomId: number;
  room: Room;
  users: number[];
}

interface BookingDto {
  id: number;
  bookingDate: string;
  status: string;
  userId: number;
  serviceId: number;
  service?: Service;
}

// Rozhraní pro statistiky
interface ReservationStatistics {
  totalMonthly: number;
  mostReservedService: string;
  mostReservedRoom: string;
  mostReservedTrainer: number | string;
}

interface RoomSummary {
  id: number;
  name: string;
}

// Přidané rozhraní pro platby z API
interface PaymentDto {
  id: number;
  userId: number;
  roleId: number;
  price: number;
  creditAmount: number;
  status: string;
  transactionType: string;
  createdAt: string;
}

enum BookingStatus {
  Confirmed = 0,
  Pending = 1,
  Cancelled = 2
}

type BmiCategory = 'Underweight' | 'Normal' | 'Overweight' | 'Obese';

type UserChartType = 'registration' | 'role' | 'status' | 'gender' | 'bmi'| 'ageGroup' ;

// Typy grafů pro finance a rezervace
type FinanceChartType = 'monthly' | 'daily' | 'status' | 'type';
type ReservationChartType = 'monthly' | 'daily' | 'status' | 'room';

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
  
  financesData: DataItem[] = [];
  reservationsData = RESERVATIONS_DATA;
  usersData = USERS_DATA;
  usersTableData: UserData[] = [];
  
  userChartType: UserChartType = 'registration';
  financeChartType: FinanceChartType = 'monthly';
  reservationChartType: ReservationChartType = 'monthly';
  
  userRoleFilter = 'all';
  userStateFilter = 'all';
  userSexFilter = 'all';
  userAgeGroupFilter = 'all';
  userRegistrationTimeFilter = 'all';
  userBmiFilter = 'all';
  userSearchFilter: string = '';

  bookingStatusFilter = 'all';
  bookingRoomFilter = 'all';
  bookingSearchFilter = '';
  bookingDateRangeStart: string | null = null;
  bookingDateRangeEnd: string | null = null;
  uniqueRooms: RoomSummary[] = [];
  
  // Filtry pro platby
  paymentStatusFilter = 'all';
  paymentTypeFilter = 'all';
  paymentSearchFilter = '';
  paymentDateRangeStart: string | null = null;
  paymentDateRangeEnd: string | null = null;
  filteredPayments: PaymentDto[] = [];
  
  isLoadingUsers = false;
  usersError: string | null = null;

  totalRevenue = 0;
  monthlyRevenue = 0;
  totalBookings = 0;
  monthlyBookings = 0;
  monthlyUsers = 0;
  
  isLoadingReservations = false;
  reservationsError: string | null = null;
  rawBookings: BookingDto[] = [];
  filteredBookings: BookingDto[] = [];
  
  // Přidané proměnné pro platby
  isLoadingPayments = false;
  paymentsError: string | null = null;
  rawPayments: PaymentDto[] = [];
  totalPaymentsCount = 0;
  totalPaymentsAmount = 0;
  monthlyPaymentsCount = 0;
  monthlyPaymentsAmount = 0;
  
  private bookingsApiUrl = environment.api.analytics + '/Bookings';
  private usersApiUrl = environment.api.analytics + '/Users';
  private paymentsApiUrl = environment.api.analytics + '/Payments';
  
  private currentDate = new Date();

  userAgeFrom: number = 0;
  userAgeTo: number = 0;
  userRegistrationFrom: string = '';  
  userRegistrationTo: string = '';

  // Přidám stav a typy pro daily analytics
  dailyAnalyticsType: 'revenue' | 'reservations' | 'newUsers' = 'revenue';
  private dailyAnalyticsTypes: Array<'revenue' | 'reservations' | 'newUsers'> = ['revenue', 'reservations', 'newUsers'];

  // Stav pro přepínání mezi stránkami v users overview panelu
  usersOverviewPage: number = 0; // 0: Users, 1: Finances, 2: Reservations
  usersOverviewPages = [
    { title: 'Users Overview' },
    { title: 'Finances Overview' },
    { title: 'Reservations Overview' }
  ];

  // Proměnné pro detail rezervace - potřebné pro vyřešení linter chyb
  selectedBooking: BookingDto | null = null;
  showDetailedReservations = false;

  // Inicializace grafu
  dailyAnalyticsChartInstance: Chart | null = null;

  constructor(private http: HttpClient) {
    this.totalRevenue = 0;
    this.monthlyRevenue = 0;
    this.totalBookings = 0;
    this.monthlyBookings = 0;
  }

  ngOnInit() {
    this.fetchReservations();
    this.fetchUsers();
    this.fetchPayments();
    this.initializeFilterOptions();
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
    this.totalRevenue = this.totalPaymentsAmount;
    this.monthlyRevenue = this.monthlyPaymentsAmount;
    
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
    
    this.http.get<BookingDto[]>(this.bookingsApiUrl).pipe(
      tap(response => console.log('Raw reservation response:', response)),
      catchError(error => {
        console.error('Error fetching reservations data:', error);
        this.reservationsError = 'Failed to load reservation data.';
        return of([]);
      }),
      finalize(() => {
        this.isLoadingReservations = false;
      })
    ).subscribe(reservations => {
      if (reservations && reservations.length > 0) {
        this.rawBookings = reservations;
        this.filteredBookings = [...this.rawBookings];
        this.initializeFilterOptions();
        this.processReservationsData(this.filteredBookings);
        this.updateDashboardMetrics();
      } else {
        console.log('No reservations found');
        this.rawBookings = [];
        this.filteredBookings = [];
        this.initializeFilterOptions();
        this.processReservationsData([]);
        this.updateDashboardMetrics();
      }
    });
  }
  
  initializeFilterOptions() {
    // Nastavím pevné, požadované místnosti
    this.uniqueRooms = [
      { id: 1, name: 'Fitness Studio A' },
      { id: 2, name: 'Yoga Room' },
      { id: 3, name: 'Fitness Hall' }
    ];
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

  usersAttending(users: number[]): number {
    return users.length;
  }
  
  capacityStatus(capacity: number, users: number[]): string {
    const freeSpots = capacity - this.usersAttending(users);
    return freeSpots + ' / ' + capacity;
  }
  
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
  
  onBookingRoomFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.bookingRoomFilter = select.value;
    this.applyReservationFilters();
  }
  
  onBookingSearchChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.bookingSearchFilter = input.value;
    this.applyReservationFilters();
  }
  
  onBookingDateFromChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.bookingDateRangeStart = input.value;
    this.applyReservationFilters();
  }
  
  onBookingDateToChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.bookingDateRangeEnd = input.value;
    this.applyReservationFilters();
  }

  resetReservationFilters() {
    this.bookingStatusFilter = 'all';
    this.bookingRoomFilter = 'all';
    this.bookingSearchFilter = '';
    this.bookingDateRangeStart = null;
    this.bookingDateRangeEnd = null;
    
    const selects = [
      'bookingStatusSelect',
      'bookingRoomSelect'
    ];
    
    selects.forEach(id => {
      const select = document.getElementById(id) as HTMLSelectElement;
      if (select) select.value = 'all';
    });
    
    const searchInput = document.getElementById('bookingSearchInput') as HTMLInputElement;
    if (searchInput) searchInput.value = '';
    
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
      
      // Room filter
      if (this.bookingRoomFilter !== 'all') {
        // Převedeme ID na číslo pro porovnávání
        const roomId = parseInt(this.bookingRoomFilter);
        if (booking.service?.roomId !== roomId) {
          return false;
        }
      }
      
      // Search filter
      if (this.bookingSearchFilter && this.bookingSearchFilter.trim() !== '') {
        const searchTerm = this.bookingSearchFilter.toLowerCase();
        const serviceName = booking.service?.serviceName.toLowerCase() || '';
        const roomName = booking.service?.room?.name.toLowerCase() || '';
        const bookingId = booking.id.toString();
        const userId = booking.userId.toString();
        
        // Hledej ve všech relevantních polích
        if (!serviceName.includes(searchTerm) && 
            !roomName.includes(searchTerm) && 
            !bookingId.includes(searchTerm) && 
            !userId.includes(searchTerm)) {
          return false;
        }
      }
      
      // Date filter
      if (!booking.bookingDate) return true;
      
      const bookingDate = new Date(booking.bookingDate);
      
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
      
      return true;
    });
    
    this.processReservationsData(this.filteredBookings);
    this.updateDashboardMetrics();
  }

  fetchUsers() {
    this.isLoadingUsers = true;
    this.usersError = null;
    
    this.http.get<UserData[]>(this.usersApiUrl).pipe(
      tap(response => console.log('Raw users response:', response)),
      catchError(error => {
        console.error('Error fetching users data:', error);
        this.usersError = 'Failed to load users data.';
        return of([]);
      }),
      finalize(() => {
        this.isLoadingUsers = false;
      })
    ).subscribe(users => {
      if (users && users.length > 0) {
        this.usersTableData = users;
        this.generateUsersChartData();
        this.initializeUserFilters();
      } else {
        console.log('No users found');
        this.usersTableData = [];
        this.generateUsersChartData();
        this.initializeUserFilters();
      }
    });
  }

  getSampleUserData(): UserData[] {
    return [];
  }

  getSampleBookingData(): BookingDto[] {
    return [];
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
      
      // Filtrování podle vyhledávacího pole
      if (this.userSearchFilter && this.userSearchFilter.trim() !== '') {
        const searchTerm = this.userSearchFilter.toLowerCase().trim();
        const userId = user.id.toString();
        const role = this.formatRole(user.roleId).toLowerCase();
        const state = this.formatState(user.state).toLowerCase();
        const gender = this.formatSex(user.sex).toLowerCase();
        
        // Vyhledáváme v různých polích uživatele
        if (!userId.includes(searchTerm) && 
            !role.includes(searchTerm) && 
            !state.includes(searchTerm) && 
            !gender.includes(searchTerm)) {
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

  onUserSearchChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.userSearchFilter = input.value;
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
    this.userSearchFilter = '';
    
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
    
    const searchInput = document.getElementById('userSearchInput') as HTMLInputElement;
    if (searchInput) searchInput.value = '';
    
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

  getCompletedPaymentsPercentage(): number {
    if (this.rawPayments.length === 0) return 0;
    const completedPayments = this.rawPayments.filter(payment => payment.status === 'Completed').length;
    return Math.round((completedPayments / this.rawPayments.length) * 100);
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
    
    let chartType: 'bar' = 'bar'; // Vždy sloupcový graf
    
    // Nastavení pro různé typy grafů
    let tension = 0.3;
    let borderWidth = 1;
    let pointRadius = 4;
    let fill = false;
    
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
    
    let datasets = [{
      label: this.getValueLabel(),
      data: data.values,
      backgroundColor: 'rgba(234, 40, 56, 0.32)',
      borderColor: 'rgb(234, 40, 56)',
      borderWidth: borderWidth,
      tension: tension,
      pointBackgroundColor: 'rgb(234, 40, 56)',
      pointRadius: pointRadius,
      fill: fill
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
        if (this.financeChartType === 'monthly') {
          return this.financesData;
        } else {
          return this.usersData;
        }
      case 'reservations':
        if (this.reservationChartType === 'monthly') {
          return this.reservationsData;
        } else {
          return this.usersData;
        }
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
      this.usersOverviewPage = 0;
      setTimeout(() => {
        this.dailyAnalyticsType = 'revenue';
        this.initializeDailyAnalyticsChart();
        this.updateCircularCharts();
      }, 0);
    } else {
      // Nastavení výchozího pohledu pro záložky - tabulka pro všechny mimo overview
      this.viewMode = 'table';
      setTimeout(() => {
        // Nastavíme výchozí typy grafů při přepnutí na záložku
        if (tab === 'finances') {
          this.financeChartType = 'monthly';
          this.generateFinancesChartData();
        } else if (tab === 'reservations') {
          this.reservationChartType = 'monthly';
          this.generateReservationsChartData();
        } else if (tab === 'users') {
          this.userChartType = 'registration';
          this.generateUsersChartData();
        }
        this.renderChart();
      }, 0);
    }
    // Reset detailed reservations view when switching tabs
    if (tab === 'reservations') {
      this.showDetailedReservations = false;
      this.resetReservationFilters();
    } else if (tab === 'users') {
      this.resetUserFilters();
    } else if (tab === 'finances') {
      this.resetPaymentFilters();
    }
  }

  toggleView() {
    this.viewMode = this.viewMode === 'chart' ? 'table' : 'chart';
    if (this.viewMode === 'chart') {
      setTimeout(() => {
        // V případě users záložky generujeme uživatelské grafy
        if (this.activeTab === 'users') {
          this.generateUsersChartData();
        } else if (this.activeTab === 'finances') {
          this.generateFinancesChartData();
        } else if (this.activeTab === 'reservations') {
          this.generateReservationsChartData();
        }
        this.renderChart();
      }, 0);
    }
  }

  getActiveTitle(): string {
    switch (this.activeTab) {
      case 'overview':
        return 'Dashboard Overview';
      case 'finances':
        return 'Finances Statistics';
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
        switch (this.financeChartType) {
          case 'monthly':
          case 'daily':
            return 'Revenue ($)';
          case 'status':
            return 'Revenue by Status ($)';
          case 'type':
            return 'Revenue by Type ($)';
          default:
            return 'Revenue ($)';
        }
      case 'reservations':
        switch (this.reservationChartType) {
          case 'monthly':
          case 'daily':
            return 'Number of Reservations';
          case 'status':
            return 'Reservations by Status';
          case 'room':
            return 'Reservations by Room';
          default:
            return 'Number of Reservations';
        }
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
        switch (this.financeChartType) {
          case 'monthly':
            return 'Month';
          case 'daily':
            return 'Day';
          case 'status':
            return 'Payment Status';
          case 'type':
            return 'Transaction Type';
          default:
            return 'Period';
        }
      case 'reservations':
        switch (this.reservationChartType) {
          case 'monthly':
            return 'Month';
          case 'daily':
            return 'Day';
          case 'status':
            return 'Reservation Status';
          case 'room':
            return 'Room';
          default:
            return 'Period';
        }
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
        switch (this.financeChartType) {
          case 'monthly':
            return 'Monthly Revenue';
          case 'daily':
            return 'Daily Revenue';
          case 'status':
            return 'Revenue by Status';
          case 'type':
            return 'Revenue by Type';
          default:
            return 'Revenue ($)';
        }
      case 'reservations':
        switch (this.reservationChartType) {
          case 'monthly':
            return 'Monthly Reservations';
          case 'daily':
            return 'Daily Reservations';
          case 'status':
            return 'Reservations by Status';
          case 'room':
            return 'Reservations by Room';
          default:
            return 'Number of Reservations';
        }
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
      case 'revenue': return 'Daily Revenues';
      case 'reservations': return 'Daily Reservations';
      case 'newUsers': return 'Daily New Users';
      default: return '';
    }
  }
  // Legenda
  getDailyAnalyticsLegend(): string {
    switch (this.dailyAnalyticsType) {
      case 'revenue': return 'Revenues';
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

  // Nová metoda pro načítání plateb
  fetchPayments() {
    this.isLoadingPayments = true;
    this.paymentsError = null;
    
    this.http.get<PaymentDto[]>(this.paymentsApiUrl).pipe(
      tap(response => console.log('Raw payments response:', response)),
      catchError(error => {
        console.error('Error fetching payments data:', error);
        this.paymentsError = 'Failed to load payments data.';
        return of([]);
      }),
      finalize(() => {
        this.isLoadingPayments = false;
      })
    ).subscribe(payments => {
      if (payments && payments.length > 0) {
        this.rawPayments = payments;
        this.filteredPayments = [...this.rawPayments];
        this.processPaymentsData(payments);
      } else {
        console.log('No payments found');
        this.rawPayments = [];
        this.filteredPayments = [];
        this.totalPaymentsAmount = 0;
        this.monthlyPaymentsAmount = 0;
      }
      
      this.updateDashboardMetrics();
      if (this.activeTab === 'overview') {
        this.initializeDailyAnalyticsChart();
      }
    });
  }
  
  // Nová metoda pro zpracování dat o platbách
  processPaymentsData(payments: PaymentDto[]) {
    this.totalPaymentsCount = payments.length;
    this.totalPaymentsAmount = payments
      .reduce((sum, payment) => sum + (payment.status === 'Completed' ? payment.price : 0), 0);
    
    // Výpočet měsíčních plateb (za poslední 30 dní)
    const thirtyDaysAgo = new Date();
    thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
    
    const recentPayments = payments.filter(p => new Date(p.createdAt) >= thirtyDaysAgo);
    this.monthlyPaymentsCount = recentPayments.length;
    this.monthlyPaymentsAmount = recentPayments
      .reduce((sum, payment) => sum + (payment.status === 'Completed' ? payment.price : 0), 0);
    
    // Zpracování dat pro graf
    const monthlyData: {[key: string]: number} = {};
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    
    months.forEach(month => {
      monthlyData[month] = 0;
    });
    
    payments.forEach(payment => {
      if (payment.createdAt && payment.status === 'Completed') {
        const date = new Date(payment.createdAt);
        const month = months[date.getMonth()];
        monthlyData[month] += payment.price;
      }
    });
    
    this.financesData = Object.keys(monthlyData)
      .map(month => ({
        date: month,
        value: monthlyData[month]
      }))
      .sort((a, b) => {
        const monthOrder = { 'Jan': 0, 'Feb': 1, 'Mar': 2, 'Apr': 3, 'May': 4, 
                          'Jun': 5, 'Jul': 6, 'Aug': 7, 'Sep': 8, 'Oct': 9, 
                          'Nov': 10, 'Dec': 11 };
        return monthOrder[a.date as keyof typeof monthOrder] - monthOrder[b.date as keyof typeof monthOrder];
      });
  }

  // Upravená metoda pro získání denních příjmů
  getDailyRevenueData(): DataItem[] {
    const now = new Date();
    const days = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    
    // Příprava pole pro všechny dny v měsíci
    const dailyRevenue: {[key: number]: number} = {};
    for (let i = 1; i <= days; i++) {
      dailyRevenue[i] = 0;
    }
    
    // Načtení všech plateb pro aktuální měsíc
    this.rawPayments.filter(payment => {
      const paymentDate = new Date(payment.createdAt);
      return paymentDate.getMonth() === now.getMonth() && 
             paymentDate.getFullYear() === now.getFullYear() &&
             payment.status === 'Completed';
    }).forEach(payment => {
      const paymentDate = new Date(payment.createdAt);
      const day = paymentDate.getDate();
      dailyRevenue[day] += payment.price;
    });
    
    // Vrácení dat ve formátu pro graf
    return Array.from({ length: days }, (_, i) => ({ 
      date: (i + 1).toString(), 
      value: dailyRevenue[i + 1] 
    }));
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

  switchUsersOverviewPage(direction: number) {
    this.usersOverviewPage += direction;
    if (this.usersOverviewPage < 0) this.usersOverviewPage = this.usersOverviewPages.length - 1;
    if (this.usersOverviewPage >= this.usersOverviewPages.length) this.usersOverviewPage = 0;
  }

  // Metody pro statistiky rezervací pro overview
  getMostReservedService(): string {
    if (this.rawBookings.length === 0) return 'N/A';
    
    const serviceCount = new Map<string, number>();
    
    this.rawBookings.forEach(booking => {
      const serviceName = booking.service?.serviceName || 'Unknown';
      serviceCount.set(serviceName, (serviceCount.get(serviceName) || 0) + 1);
    });
    
    let maxCount = 0;
    let mostReservedService = 'N/A';
    
    serviceCount.forEach((count, name) => {
      if (count > maxCount) {
        maxCount = count;
        mostReservedService = name;
      }
    });
    
    return mostReservedService;
  }
  
  getMostReservedRoom(): string {
    if (this.rawBookings.length === 0) return 'N/A';
    
    const roomCount = new Map<string, number>();
    
    this.rawBookings.forEach(booking => {
      const roomName = booking.service?.room?.name || 'Unknown';
      roomCount.set(roomName, (roomCount.get(roomName) || 0) + 1);
    });
    
    let maxCount = 0;
    let mostReservedRoom = 'N/A';
    
    roomCount.forEach((count, name) => {
      if (count > maxCount) {
        maxCount = count;
        mostReservedRoom = name;
      }
    });
    
    return mostReservedRoom;
  }
  
  getMostReservedTrainer(): string {
    if (this.rawBookings.length === 0) return 'N/A';
    
    const trainerCount = new Map<number, number>();
    
    this.rawBookings.forEach(booking => {
      const trainerId = booking.service?.trainerId || 0;
      if (trainerId) {
        trainerCount.set(trainerId, (trainerCount.get(trainerId) || 0) + 1);
      }
    });
    
    let maxCount = 0;
    let mostReservedTrainer = 0;
    
    trainerCount.forEach((count, id) => {
      if (count > maxCount) {
        maxCount = count;
        mostReservedTrainer = id;
      }
    });
    
    return mostReservedTrainer.toString();
  }

  // Metoda pro mazání vyhledávacího pole uživatelů
  clearUserSearch() {
    this.userSearchFilter = '';
    const searchInput = document.getElementById('userSearchInput') as HTMLInputElement;
    if (searchInput) searchInput.value = '';
    this.generateUsersChartData();
    
    if (this.viewMode === 'chart') {
      this.renderChart();
    }
  }

  // Metoda pro mazání vyhledávacího pole rezervací
  clearBookingSearch() {
    this.bookingSearchFilter = '';
    const searchInput = document.getElementById('bookingSearchInput') as HTMLInputElement;
    if (searchInput) searchInput.value = '';
    this.applyReservationFilters();
  }
  
  // Upravená metoda exportToCsv, která volá specifickou metodu dle aktuální záložky
  exportToCsv() {
    if (this.activeTab === 'users') {
      // Použijeme data z backend API, pokud jsou k dispozici
      const users = this.getFilteredUsers();
      if (users.length === 0) return;
      
      // Hlavička CSV
      let csvContent = 'ID,Role,Status,Gender,Weight (kg),Height (cm),BMI,Birth Date,Age,Registered,Last Updated\n';
      
      // Data řádků
      users.forEach(user => {
        const bmi = this.calculateBMI(user.weight, user.height);
        const row = [
          user.id,
          this.formatRole(user.roleId),
          this.formatState(user.state),
          this.formatSex(user.sex),
          user.weight,
          user.height,
          this.formatBMI(bmi),
          user.birthDate.split('T')[0], // Format date as YYYY-MM-DD
          this.calculateAge(user.birthDate),
          user.createdAt.split('T')[0], // Format date as YYYY-MM-DD
          user.lastUpdated.split('T')[0] // Format date as YYYY-MM-DD
        ];
        
        // Escape any commas in text fields and wrap with quotes if needed
        csvContent += row.map(cell => {
          const cellStr = String(cell);
          return cellStr.includes(',') ? `"${cellStr}"` : cellStr;
        }).join(',') + '\n';
      });
      
      // Vytvoření a stažení souboru
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      const url = URL.createObjectURL(blob);
      
      link.setAttribute('href', url);
      link.setAttribute('download', 'users_export.csv');
      link.style.visibility = 'hidden';
      
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
    else if (this.activeTab === 'reservations') {
      // Použijeme data z backend API, pokud jsou k dispozici
      const reservations = this.rawBookings.length > 0 ? this.filteredBookings : [];
      if (reservations.length === 0) return;
      
      // Hlavička CSV
      let csvContent = 'ID,User ID,Trainer ID,Service,Room,Capacity,Users,Booking Date,Status\n';
      
      // Data řádků
      reservations.forEach(booking => {
        const row = [
          booking.id,
          booking.userId,
          booking.service?.trainerId || 'N/A',
          booking.service?.serviceName || booking.serviceId || 'N/A',
          booking.service?.room?.name || 'N/A',
          booking.service?.room?.capacity && booking.service?.users ? 
            `${this.capacityStatus(booking.service.room.capacity, booking.service.users)}` : 'N/A',
          booking.service?.users ? this.usersAttending(booking.service.users) : 'N/A',
          booking.bookingDate ? booking.bookingDate.split('T')[0] : 'N/A', // Format date as YYYY-MM-DD
          booking.status || 'Unknown'
        ];
        
        // Escape any commas in text fields and wrap with quotes if needed
        csvContent += row.map(cell => {
          const cellStr = String(cell);
          return cellStr.includes(',') ? `"${cellStr}"` : cellStr;
        }).join(',') + '\n';
      });
      
      // Vytvoření a stažení souboru
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      const url = URL.createObjectURL(blob);
      
      link.setAttribute('href', url);
      link.setAttribute('download', 'reservations_export.csv');
      link.style.visibility = 'hidden';
      
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
    else if (this.activeTab === 'finances') {
      this.exportPaymentsToCsv();
    }
  }

  // Filtrovací metody pro platby
  onPaymentStatusFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.paymentStatusFilter = select.value;
    this.applyPaymentFilters();
  }
  
  onPaymentTypeFilterChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.paymentTypeFilter = select.value;
    this.applyPaymentFilters();
  }
  
  onPaymentSearchChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.paymentSearchFilter = input.value;
    this.applyPaymentFilters();
  }
  
  onPaymentDateFromChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.paymentDateRangeStart = input.value;
    this.applyPaymentFilters();
  }
  
  onPaymentDateToChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.paymentDateRangeEnd = input.value;
    this.applyPaymentFilters();
  }
  
  resetPaymentFilters() {
    this.paymentStatusFilter = 'all';
    this.paymentTypeFilter = 'all';
    this.paymentSearchFilter = '';
    this.paymentDateRangeStart = null;
    this.paymentDateRangeEnd = null;
    
    const selects = [
      'paymentStatusSelect',
      'paymentTypeSelect'
    ];
    
    selects.forEach(id => {
      const select = document.getElementById(id) as HTMLSelectElement;
      if (select) select.value = 'all';
    });
    
    const searchInput = document.getElementById('paymentSearchInput') as HTMLInputElement;
    if (searchInput) searchInput.value = '';
    
    this.filteredPayments = [...this.rawPayments];
    
    if (this.activeTab === 'finances' && this.viewMode === 'chart') {
      setTimeout(() => this.renderChart(), 100);
    }
  }
  
  applyPaymentFilters() {
    this.filteredPayments = this.rawPayments.filter(payment => {
      // Status filter
      if (this.paymentStatusFilter !== 'all') {
        if (payment.status.toLowerCase() !== this.paymentStatusFilter.toLowerCase()) {
          return false;
        }
      }
      
      // Type filter
      if (this.paymentTypeFilter !== 'all') {
        if (payment.transactionType.toLowerCase() !== this.paymentTypeFilter.toLowerCase()) {
          return false;
        }
      }
      
      // Search filter
      if (this.paymentSearchFilter && this.paymentSearchFilter.trim() !== '') {
        const searchTerm = this.paymentSearchFilter.toLowerCase();
        const paymentId = payment.id.toString();
        const userId = payment.userId.toString();
        const price = payment.price.toString();
        const creditAmount = payment.creditAmount.toString();
        
        // Hledej ve všech relevantních polích
        if (!paymentId.includes(searchTerm) && 
            !userId.includes(searchTerm) && 
            !price.includes(searchTerm) && 
            !creditAmount.includes(searchTerm)) {
          return false;
        }
      }
      
      // Date filter
      if (payment.createdAt) {
        const paymentDate = new Date(payment.createdAt);
        
        if (this.paymentDateRangeStart) {
          const startDate = new Date(this.paymentDateRangeStart);
          if (paymentDate < startDate) {
            return false;
          }
        }
        
        if (this.paymentDateRangeEnd) {
          const endDate = new Date(this.paymentDateRangeEnd);
          endDate.setHours(23, 59, 59, 999); // End of day
          if (paymentDate > endDate) {
            return false;
          }
        }
      }
      
      return true;
    });
  }
  
  clearPaymentSearch() {
    this.paymentSearchFilter = '';
    const searchInput = document.getElementById('paymentSearchInput') as HTMLInputElement;
    if (searchInput) searchInput.value = '';
    this.applyPaymentFilters();
  }
  
  // Export metoda pro platby
  exportPaymentsToCsv() {
    if (this.filteredPayments.length === 0) return;
    
    // Hlavička CSV
    let csvContent = 'ID,User ID,Role,Price,Credits,Status,Type,Date\n';
    
    // Data řádků
    this.filteredPayments.forEach(payment => {
      const row = [
        payment.id,
        payment.userId,
        this.formatRole(payment.roleId),
        payment.price,
        payment.creditAmount,
        payment.status,
        payment.transactionType,
        payment.createdAt ? payment.createdAt.split('T')[0] : 'N/A' // Format date as YYYY-MM-DD
      ];
      
      // Escape any commas in text fields and wrap with quotes if needed
      csvContent += row.map(cell => {
        const cellStr = String(cell);
        return cellStr.includes(',') ? `"${cellStr}"` : cellStr;
      }).join(',') + '\n';
    });
    
    // Vytvoření a stažení souboru
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', 'payments_export.csv');
    link.style.visibility = 'hidden';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  onFinanceChartTypeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.financeChartType = select.value as FinanceChartType;
    this.generateFinancesChartData();
    this.renderChart();
  }

  onReservationChartTypeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.reservationChartType = select.value as ReservationChartType;
    this.generateReservationsChartData();
    this.renderChart();
  }

  // Metoda pro generování dat grafu financí podle zvoleného typu
  generateFinancesChartData() {
    switch (this.financeChartType) {
      case 'monthly':
        // Použiju stávající data, která jsou už připravena po fetchPayments()
        break;
      case 'daily':
        this.usersData = this.getDailyRevenueData();
        break;
      case 'status':
        this.generateStatusFinanceChartData();
        break;
      case 'type':
        this.generateTypeFinanceChartData();
        break;
    }
  }

  // Metoda pro generování dat grafu rezervací podle zvoleného typu
  generateReservationsChartData() {
    switch (this.reservationChartType) {
      case 'monthly':
        // Použiju stávající data, která jsou už připravena v reservationsData
        break;
      case 'daily':
        this.usersData = this.getDailyReservationsData();
        break;
      case 'status':
        this.generateStatusReservationChartData();
        break;
      case 'room':
        this.generateRoomReservationChartData();
        break;
    }
  }

  // Generování dat pro grafy financí podle statusů
  generateStatusFinanceChartData() {
    const statusCount: { [key: string]: number } = {
      'Completed': 0,
      'Pending': 0,
      'Failed': 0
    };
    
    this.rawPayments.forEach(payment => {
      statusCount[payment.status] = (statusCount[payment.status] || 0) + payment.price;
    });
    
    this.usersData = Object.keys(statusCount).map(status => ({
      date: status,
      value: statusCount[status]
    }));
  }

  // Generování dat pro grafy financí podle typů transakce
  generateTypeFinanceChartData() {
    const typeCount: { [key: string]: number } = {};
    
    this.rawPayments.forEach(payment => {
      typeCount[payment.transactionType] = (typeCount[payment.transactionType] || 0) + payment.price;
    });
    
    this.usersData = Object.keys(typeCount).map(type => ({
      date: type,
      value: typeCount[type]
    }));
  }

  // Generování dat pro grafy rezervací podle statusů
  generateStatusReservationChartData() {
    const statusCount: { [key: string]: number } = {
      'Confirmed': 0,
      'Pending': 0,
      'Cancelled': 0
    };
    
    this.rawBookings.forEach(booking => {
      statusCount[booking.status] = (statusCount[booking.status] || 0) + 1;
    });
    
    this.usersData = Object.keys(statusCount).map(status => ({
      date: status,
      value: statusCount[status]
    }));
  }

  // Generování dat pro grafy rezervací podle místností
  generateRoomReservationChartData() {
    const roomCount: { [key: string]: number } = {};
    
    this.rawBookings.forEach(booking => {
      const roomName = booking.service?.room?.name || 'Unknown';
      roomCount[roomName] = (roomCount[roomName] || 0) + 1;
    });
    
    this.usersData = Object.keys(roomCount).map(room => ({
      date: room,
      value: roomCount[room]
    }));
  }
}