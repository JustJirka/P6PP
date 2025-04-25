import React from 'react';
import { Card } from 'react-bootstrap';

const UserDetailCard = ({ title, children, className = '' }) => {
  return (
    <Card className={`mb-4 ${className}`}>
      {title && <Card.Header as="h5">{title}</Card.Header>}
      <Card.Body>{children}</Card.Body>
    </Card>
  );
};

export default UserDetailCard;