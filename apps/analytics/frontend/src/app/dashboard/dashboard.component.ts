import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

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

// Prázdné pole pro uživatele - bude implementováno později
const USERS_DATA: DataItem[] = [];

// Prázdné pole pro trenéry - bude implementováno později
const TRAINERS_DATA: DataItem[] = [];

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

  // Data pro jednotlivé záložky
  overviewData = OVERVIEW_DATA;
  financesData = FINANCES_DATA;
  reservationsData = RESERVATIONS_DATA;
  roomsData = ROOMS_DATA;
  usersData = USERS_DATA;
  trainersData = TRAINERS_DATA;

  yAxisValues: number[] = [];

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
        return 'Active Users';
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
}
