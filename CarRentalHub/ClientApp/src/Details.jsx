import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom';
import Calendar from './Calendar';

const Details = ({ carId }) => {
    const [selectedDate, setSelectedDate] = useState(null);

    const handleDateSelect = (date) => {
        setSelectedDate(date);
    };

    const handleReservationSubmit = () => {
        // Tu możesz wysłać zapytanie do serwera w celu zarezerwowania terminu.
        console.log(`Rezerwacja dla samochodu o ID ${carId}: ${selectedDate}`);
    };

    return (
        <div>
            <h2>Rezerwacja Samochodu</h2>
            <div>
                <Calendar onDateSelect={handleDateSelect} />
                <button onClick={handleReservationSubmit}>Zarezerwuj</button>
            </div>
        </div>
    );
};

// Pobierz ID samochodu z atrybutu data-car-id
const carId = document.getElementById('reservation-app').dataset.carId;

ReactDOM.render(<Details carId={carId} />, document.getElementById('reservation-app'));
