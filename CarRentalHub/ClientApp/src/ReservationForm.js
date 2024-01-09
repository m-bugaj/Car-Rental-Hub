// ClientApp/src/ReservationForm.js
import React, { useState } from 'react';

function ReservationForm({ selectedDate }) {
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        console.log(`Rezerwacja na ${selectedDate}: ${name}, ${email}`);
        // Tutaj możesz dodać logikę obsługi rezerwacji, np. wysłać dane na serwer.
    };

    return (
        <div>
            <h2>Formularz Rezerwacji</h2>
            {selectedDate && (
                <form onSubmit={handleSubmit}>
                    <label>Imię:</label>
                    <input type="text" value={name} onChange={(e) => setName(e.target.value)} />
                    <label>Email:</label>
                    <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
                    <button type="submit">Zarezerwuj</button>
                </form>
            )}
        </div>
    );
}

export default ReservationForm;
