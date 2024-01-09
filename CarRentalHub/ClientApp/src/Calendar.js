import React from 'react';
import CalendarReact from 'react-calendar';
import 'react-calendar/dist/Calendar.css';

function Calendar({ onDateSelect }) {
    return (
        <div>
            <h2>Wybierz Datę</h2>
            <CalendarReact onChange={onDateSelect} />
        </div>
    );
}

export default Calendar;
