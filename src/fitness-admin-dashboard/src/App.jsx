import { Routes, Route, Navigate } from 'react-router-dom';
import LoginForm from './auth/LoginForm';
import AdminLayout from './admin/AdminLayout';
import Dashboard from './admin/Dashboard';
import Users from './lists/Users';
import ViewUser from './lists/pages/ViewUser';
import EditUser from './lists/pages/EditUser';
import CreateUser from './lists/pages/CreateUser';
import CreateRole from './lists/pages/CreateRoles';
import Roles from './lists/pages/Roles';
import Rooms from './lists/pages/Rooms';
import ViewRoom from './lists/pages/ViewRoom';
import EditRoom from './lists/pages/EditRoom';
import CreateRoom from './lists/pages/CreateRoom';
import Bookings from './lists/pages/Bookings';
import CreateBooking from './lists/pages/CreateBooking';
import Services from './lists/pages/Services';
import CreateService from './lists/pages/CreateService';
import EditService from './lists/pages/EditService';
import ViewService from './lists/pages/ViewService';
import DashboardEmailTemplates from './admin/Dashboard_EmailTemplates';
import ProtectedRoute from './auth/ProtectedRoute'

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginForm />} />

      <Route
        path="/admin"
        element={
          <ProtectedRoute>
            <AdminLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Navigate to="dashboard" replace />} />
        <Route path="dashboard" element={<Dashboard />} />
        <Route path="users" element={<Users />} />
        <Route path="users/view/:userId" element={<ViewUser />} />
        <Route path="users/edit/:userId" element={<EditUser />} />
        <Route path="users/create" element={<CreateUser />} />
        <Route path="roles" element={<Roles />} />
        <Route path="roles/create" element={<CreateRole />} />
        <Route path="rooms" element={<Rooms />} />
        <Route path="rooms/create" element={<CreateRoom />} />
        <Route path="rooms/edit/:roomId" element={<EditRoom />} />
        <Route path="rooms/view/:roomId" element={<ViewRoom />} />
        <Route path="bookings" element={<Bookings />} />
        <Route path="bookings/create" element={<CreateBooking />} />
        <Route path="services" element={<Services />} />
        <Route path="services/create" element={<CreateService />} />
        <Route path="services/edit/:serviceId" element={<EditService />} />
        <Route path="services/view/:serviceId" element={<ViewService />} />
        <Route path="emailtemplates" element={<DashboardEmailTemplates />} />
      </Route>

      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;
