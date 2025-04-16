import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart } from 'chart.js/auto';

// Data models
interface DataItem {
  date: string;
  value: number;
}

interface RoomData {
  roomId: string;
  roomName: string;
  occupancy: DataItem[];
}

// Mock data
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
export class DashboardComponent implements OnInit {
  // Expose Math to the template
  Math = Math;
  
  activeTab: 'overview' | 'finances' | 'reservations' | 'rooms' | 'users' = 'overview';
  viewMode: 'chart' | 'table' = 'chart';
  selectedRoom: string | null = null;
  chartInstance: Chart | null = null;
  
  // Data for tabs
  financesData = FINANCES_DATA;
  reservationsData = RESERVATIONS_DATA;
  roomsData = ROOMS_DATA;
  usersData = USERS_DATA;

  // Stats for overview cards
  totalRevenue = this.getTotalFinanceValue();
  monthlyRevenue = this.getLatestFinanceValue();
  totalBookings = this.getTotalReservationsValue();
  monthlyBookings = this.getLatestReservationsValue();
  averageOccupancy = this.getAverageRoomOccupancyValue();
  highestOccupancyRoom = this.getHighestOccupancyRoom();

  ngOnInit() {
    this.initializeChart();
  }

  initializeChart() {
    setTimeout(() => {
      this.renderChart();
    }, 0);
  }

  renderChart() {
    const canvas = document.getElementById('dataChart') as HTMLCanvasElement;
    if (!canvas) return;
    
    if (this.chartInstance) {
      this.chartInstance.destroy();
    }
    
    const data = this.getChartData();
    
    this.chartInstance = new Chart(canvas, {
      type: 'line',
      data: {
        labels: data.labels,
        datasets: [{
          label: this.getValueLabel(),
          data: data.values,
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
        return this.financesData; // Default to showing finances on overview
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
    if (tab !== 'overview') {
      setTimeout(() => {
        this.renderChart();
      }, 0);
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

  getValueLabel(): string {
    switch (this.activeTab) {
      case 'finances':
        return 'Revenue ($)';
      case 'reservations':
        return 'Number of Reservations';
      case 'rooms':
        return 'Occupancy (%)';
      case 'users':
        return 'Active Users';
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

  // Calculate difference between current and previous value
  getValueDifference(current: number, previous: number): number {
    return Math.abs(current - previous);
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

  // Calculate average room occupancy
  getAverageRoomOccupancyValue(): number {
    const allOccupancies = this.roomsData.flatMap(room => room.occupancy);
    if (allOccupancies.length === 0) return 0;
    
    const total = allOccupancies.reduce((sum, item) => sum + item.value, 0);
    return Math.round(total / allOccupancies.length);
  }

  // Find the room with highest average occupancy
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
}