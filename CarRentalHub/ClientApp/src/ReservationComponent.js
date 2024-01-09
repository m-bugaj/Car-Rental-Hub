import React, { useState, useEffect } from 'react';

const ReservationComponent = ({ carId }) => {
    const [availableDays, setAvailableDays] = useState([]);
    const [selectedDay, setSelectedDay] = useState(null);

    useEffect(() => {
        // Pobierz dostępne dni dla danego auta z serwera ASP.NET Core
        fetch(`/api/reservation/${carId}/available-days`)
            .then(response => response.json())
            .then(data => setAvailableDays(data));
    }, [carId]);

    const handleDayClick = (day) => {
        // Obsłuż kliknięcie na dzień kalendarza
        setSelectedDay(day);
    };

    const handleReservation = () => {
        // Wyślij żądanie rezerwacji do serwera ASP.NET Core
        fetch(`/api/reservation/${carId}/reserve`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ selectedDay }),
        })
            .then(response => {
                if (response.ok) {
                    // Rezerwacja udana, możesz wykonać dodatkowe działania
                } else {
                    // Rezerwacja nieudana, obsłuż błąd
                }
            });
    };

    return (
        <div>
            <h2>Reservation</h2>
            <div>
                {/* Wyświetl kalendarz z dostępnymi dniami */}
                {/* Możesz użyć popularnej biblioteki kalendarza, np. react-calendar */}
                {/* Poniżej przykład użycia react-calendar */}
                <Calendar onClickDay={handleDayClick} />
            </div>
            <div>
                {/* Wyświetl przycisk rezerwacji */}
                <button onClick={handleReservation} disabled={!selectedDay}>
                    Reserve
                </button>
            </div>
        </div>
    );
};

export default ReservationComponent;
