import React, { useState } from 'react';
import Calendar from './Calendar';
import ReservationForm from './ReservationForm';

function App() {
    const [selectedDate, setSelectedDate] = useState(null);

    const handleDateSelect = (date) => {
        setSelectedDate(date);
    };

    return (
        <div className="App">
            <h1>System Rezerwacji</h1>
            <div className="container">
                <div className="column">
                    <Calendar onDateSelect={handleDateSelect} />
                </div>
                <div className="column">
                    <ReservationForm selectedDate={selectedDate} />
                </div>
            </div>
        </div>
    );
}

export default App;
