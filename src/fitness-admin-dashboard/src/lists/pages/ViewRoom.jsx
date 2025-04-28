import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Spinner, Alert, Card } from 'react-bootstrap';
import Api from '../../utils/Api';

const ViewRoom = () => {
  const { roomId } = useParams();
  const [room, setRoom] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRoom = async () => {
      try {
        const data = await Api.get(`/booking-api/api/Rooms/${roomId}`);
        console.log('Room from API:', data);
        setRoom(data.data);
      } catch (err) {
        setError(`Error with loading room: ${err.message}`);
      }
    };

    fetchRoom();
  }, [roomId]);

  if (error) return <Alert variant="danger">{error}</Alert>;
  if (!room) return <Spinner animation="border" />;

  return (
    <div>
      <h2>Room detail</h2>
      <Card>
        <Card.Body>
          <p><strong>ID:</strong> {room.id}</p>
          <p><strong>Name:</strong> {room.name}</p>
          <p><strong>Capacity:</strong> {room.capacity}</p>
          <p><strong>Status:</strong> {room.status}</p>
        </Card.Body>
      </Card>
    </div>
  );
};

export default ViewRoom;
