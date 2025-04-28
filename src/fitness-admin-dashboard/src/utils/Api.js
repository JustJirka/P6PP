class Api {
  static async request(method, endpoint, body = null) {
    const options = {
      method,
      headers: {},
    };
  
    const token = localStorage.getItem('token');
    if (token) {
      options.headers['Authorization'] = `Bearer ${token}`;
    }
  
    if (body) {
      options.headers['Content-Type'] = 'application/json';
      options.body = JSON.stringify(body);
    }
  
    const response = await fetch(endpoint, options);
    const status = response.status;
    const contentType = response.headers.get('Content-Type');

    if (status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
      return;
    }
  
    let data;
    try {
      if (status !== 204 && contentType && contentType.includes('application/json')) {
        data = await response.json();
      } else if (status !== 204 && contentType?.includes('text/plain')) {
        const text = await response.text();
        if (!response.ok) {
          throw new Error(text || `Unexpected response with status ${status}`);
        }
        return { message: text };
      }
  
      if (!response.ok) {
        console.error('Backend error response:', data);
        const message = data?.message || `Error ${status}`;
        throw new Error(message);
      }
  
      return data;
    } catch (error) {
      console.error('API request error:', error, 'Response:', response);
      throw error;
    }
  }
  

  // Původní obecné metody
  static get(endpoint) {
    return this.request('GET', endpoint);
  }

  static post(endpoint, body) {
    return this.request('POST', endpoint, body);
  }

  static put(endpoint, body) {
    return this.request('PUT', endpoint, body);
  }

  static delete(endpoint) {
    return this.request('DELETE', endpoint);
  }

  static getRooms() {
    return this.get('/booking-api/api/Rooms');
  }

  static createRoom(room) {
    return this.post('/booking-api/api/Rooms', room);
  }

  static updateRoom(roomId, room) {
    return this.put(`/booking-api/api/Rooms/${roomId}`, room);
  }

  static deleteRoom(roomId) {
    return this.delete(`/booking-api/api/Rooms/${roomId}`);
  }

  static getBookings() {
    return this.get('/booking-api/api/Bookings');
  }
  
  static createBooking(booking) {
    return this.post('/booking-api/api/Bookings', booking);
  }
  
  static deleteBooking(bookingId) {
    return this.delete(`/booking-api/api/Bookings/${bookingId}`);
  }

  static getServices() {
    return this.get('/booking-api/api/Services');
  }

  static createService(service) {
    return this.post('/booking-api/api/Services', service);
  }

  static updateService(service) {
    return this.put('/booking-api/api/Services', service);
  }

  static deleteService(serviceId) {
    return this.delete(`/booking-api/api/Services/${serviceId}`);
  }

  static async getRoles() {
    return this.get('/api/roles/');
  }

  static async changeUserRole(userId, roleId) {
    return this.post('/api/user/role', { userId, roleId });
  }
}

export default Api;
