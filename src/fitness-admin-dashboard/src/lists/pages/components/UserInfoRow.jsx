import React from 'react';
import { Col } from 'react-bootstrap';

const UserInfoRow = ({ label, value, colSize = { xs: 12, md: 6, lg: 3 }, className = '' }) => {
  const colProps = { ...colSize };
  return (
    <Col {...colProps} className={`mb-3 ${className}`}>
      <div className="text-muted small">{label}</div>
      <div>{value}</div>
    </Col>
  );
};

export default UserInfoRow;
