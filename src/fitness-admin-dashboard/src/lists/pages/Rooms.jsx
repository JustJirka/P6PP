import React, { useEffect, useState } from 'react';
import { Table, Button, Alert, Spinner, Dropdown, ButtonGroup } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Api from "../../utils/Api";

const Rooms = () => {
  const navigate = useNavigate();
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        setLoading(true);
        const res = await Api.getRooms(); 
        const roomList = res?.data || []; 
        setRooms(roomList);
        setError(null);
      } catch (err) {
        setError('Room list couldnt be loaded. Please try again later.');
      } finally {
        setLoading(false);
      }
    };
  
    fetchRooms();
  }, []);
  

  const handleCreate = () => {
    navigate('/admin/rooms/create');
  };

  const handleEdit = (id) => {
    navigate(`/admin/rooms/edit/${id}`);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Do you really want to delete this room?')) return;
  
    try {
      await Api.deleteRoom(id);
      setRooms(prev => prev.filter(room => room.id !== id));
    } catch (err) {
      alert(`Error with deleting the room: ${err.message}`);
    }
  };  

  const handleRowClick = (id, e) => {
    if (e.target.closest('.dropdown')) return;
    navigate(`/admin/rooms/view/${id}`);
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Rooms</h2>
        <Button variant="primary" onClick={handleCreate}>+ Create room</Button>
      </div>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading ? (
        <div className="text-center my-5">
          <Spinner animation="border" role="status" />
        </div>
      ) : (
        <Table hover responsive bordered className="align-middle">
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Capacity</th>
              <th>Status</th>
              <th style={{ width: '60px' }}></th>
            </tr>
          </thead>
          <tbody>
            {rooms.length === 0 ? (
              <tr>
                <td colSpan="5" className="text-center">No rooms were found.</td>
              </tr>
            ) : (
              rooms.map(room => (
                <tr
                  key={room.id}
                  onClick={(e) => handleRowClick(room.id, e)}
                  style={{ cursor: 'pointer' }}
                >
                  <td>{room.id}</td>
                  <td>{room.name}</td>
                  <td>{room.capacity}</td>
                  <td>{room.status}</td>
                  <td>
                    <Dropdown as={ButtonGroup} className="dropdown">
                      <Dropdown.Toggle variant="light" size="sm" className="border-0">⋯</Dropdown.Toggle>
                      <Dropdown.Menu align="end">
                        <Dropdown.Item onClick={() => handleEdit(room.id)}>Edit</Dropdown.Item>
                        <Dropdown.Item onClick={() => handleDelete(room.id)} className="text-danger">Delete</Dropdown.Item>
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

export default Rooms;