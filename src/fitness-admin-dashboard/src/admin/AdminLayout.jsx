import { Outlet, Link } from 'react-router-dom';
import { Container, Row, Col, Nav } from 'react-bootstrap';

const AdminLayout = () => {
  return (
    <Container fluid className="vh-100">
      <Row className="h-100">
        <Col md={2} className="bg-light p-3 border-end">
          <h5 className="mb-4">Admin Menu</h5>
          <Nav className="flex-column">
            <Nav.Link as={Link} to="/admin/dashboard">Dashboard</Nav.Link>
            <Nav.Link as={Link} to="/admin/users">Users</Nav.Link>
            <Nav.Link as={Link} to="/admin/roles">Roles</Nav.Link>
            <Nav.Link as={Link} to="/admin/rooms">Rooms</Nav.Link>
            <Nav.Link as={Link} to="/admin/bookings">Booking</Nav.Link>
            <Nav.Link as={Link} to="/admin/services">Services</Nav.Link>
          </Nav>
        </Col>
        <Col md={10} className="p-4">
          <Outlet />
        </Col>
      </Row>
    </Container>
  );
};

export default AdminLayout;