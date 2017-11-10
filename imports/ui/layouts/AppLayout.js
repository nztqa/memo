import React from 'react';
import Header from '../components/Header';

export default class AppLayout extends React.Component {
  render() {
    return (
      <div className="container">
        <Header />
      </div>
    );
  }
}
