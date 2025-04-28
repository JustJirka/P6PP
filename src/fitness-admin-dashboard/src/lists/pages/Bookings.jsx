import React, { useEffect, useState } from 'react';
import { Table, Button, Spinner, Alert, Dropdown, ButtonGroup } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Api from '../../utils/Api';

const Bookings = () => {
  const navigate = useNavigate();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchBookings = async () => {
    try {
        const res = await Api.getBookings();
      setBookings(res.data || []);
    } catch (err) {
      setError('Failed to load reservations.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBookings();
  }, []);

  const handleDelete = async (id) => {
    if (!window.confirm('Do you really want to delete this reservation?')) return;

    try {
        await Api.deleteBooking(id);
      setBookings(prev => prev.filter(b => b.id !== id));
    } catch (err) {
      alert(`Error with deleting booking: ${err.message}`);
    }
  };

  const handleCreate = () => {
    navigate('/admin/bookings/create');
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>My bookings</h2>
        <Button onClick={handleCreate}>+ New booking</Button>
      </div>

      {error && <Alert variant="danger">{error}</Alert>}
      {loading ? (
        <Spinner animation="border" />
      ) : (
        <Table striped bordered hover>
          <thead>
            <tr>
              <th>ID</th>
              <th>Service ID</th>
              <th>Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {bookings.length === 0 ? (
              <tr>
                <td colSpan="4" className="text-center">No bookings available</td>
              </tr>
            ) : (
              bookings.map(b => (
                <tr key={b.id}>
                  <td>{b.id}</td>
                  <td>{b.serviceId}</td>
                  <td>{b.status}</td>
                  <td>
                    <Dropdown as={ButtonGroup}>
                      <Dropdown.Toggle variant="light" size="sm" className="border-0">⋯</Dropdown.Toggle>
                      <Dropdown.Menu align="end">
                        <Dropdown.Item onClick={() => handleDelete(b.id)} className="text-danger">
                          Delete
                        </Dropdown.Item>
                      </Dropdown.Menu>
                    </Dropdown>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </Table>
      )}
    </div>
  );
};

export default Bookings;
