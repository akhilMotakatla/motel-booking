import React, { useEffect, useState } from 'react';
import axios from 'axios';

const UserDashboard = () => {
  const token = localStorage.getItem('userToken');
  const [user, setUser] = useState(null);
  const [rooms, setRooms] = useState([]);
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [profileEdit, setProfileEdit] = useState(false);
  const [profileData, setProfileData] = useState({});

  // Fetch user info, rooms, and bookings on mount
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [userRes, roomsRes, bookingsRes] = await Promise.all([
          axios.get('http://localhost:5000/api/users/me', { headers: { Authorization: `Bearer ${token}` }}),
          axios.get('http://localhost:5000/api/rooms'),
          axios.get('http://localhost:5000/api/bookings/user', { headers: { Authorization: `Bearer ${token}` }})
        ]);
        setUser(userRes.data);
        setProfileData(userRes.data);
        setRooms(roomsRes.data);
        setBookings(bookingsRes.data);
        setLoading(false);
      } catch (err) {
        console.error(err);
        setLoading(false);
      }
    };

    fetchData();
  }, [token]);

  // Profile update handler
  const handleProfileChange = (e) => {
    setProfileData({...profileData, [e.target.name]: e.target.value});
  };

  const updateProfile = async () => {
    try {
      await axios.put('http://localhost:5000/api/users/me', profileData, { headers: { Authorization: `Bearer ${token}` }});
      setUser(profileData);
      setProfileEdit(false);
      alert('Profile updated successfully!');
    } catch (err) {
      alert('Failed to update profile.');
    }
  };

  // Booking handler (simplified)
  const bookRoom = async (roomId) => {
    const checkInDate = prompt('Enter check-in date (YYYY-MM-DD)');
    const checkOutDate = prompt('Enter check-out date (YYYY-MM-DD)');
    // For production, use proper forms & validation!

    if (!checkInDate || !checkOutDate) return alert('Please provide check-in and check-out dates');

    try {
      await axios.post('http://localhost:5000/api/bookings', {
        roomId,
        checkInDate,
        checkOutDate
      }, { headers: { Authorization: `Bearer ${token}` }});
      alert('Booking successful!');
      // Refresh bookings
      const bookingsRes = await axios.get('http://localhost:5000/api/bookings/user', { headers: { Authorization: `Bearer ${token}` }});
      setBookings(bookingsRes.data);
    } catch (err) {
      alert('Booking failed.');
    }
  };

  if (loading) return <p>Loading...</p>;

  return (
    <div className="dashboard">
      <h2>Welcome, {user.fullName}</h2>

      <section className="profile-section">
        <h3>Your Profile</h3>
        {profileEdit ? (
          <>
            <input name="fullName" value={profileData.fullName} onChange={handleProfileChange} />
            <input name="email" value={profileData.email} onChange={handleProfileChange} />
            <input name="address" value={profileData.address || ''} onChange={handleProfileChange} />
            <input name="city" value={profileData.city || ''} onChange={handleProfileChange} />
            <input name="state" value={profileData.state || ''} onChange={handleProfileChange} />
            <input name="zipcode" value={profileData.zipcode || ''} onChange={handleProfileChange} />
            <input name="contact" value={profileData.contact || ''} onChange={handleProfileChange} />
            <button onClick={updateProfile}>Save</button>
            <button onClick={() => setProfileEdit(false)}>Cancel</button>
          </>
        ) : (
          <>
            <p><strong>Name:</strong> {user.fullName}</p>
            <p><strong>Email:</strong> {user.email}</p>
            <p><strong>Address:</strong> {user.address}, {user.city}, {user.state} {user.zipcode}</p>
            <p><strong>Contact:</strong> {user.contact}</p>
            <button onClick={() => setProfileEdit(true)}>Edit Profile</button>
          </>
        )}
      </section>

      <section className="rooms-section">
        <h3>Available Rooms</h3>
        <div className="room-list">
          {rooms.map(room => (
            <div key={room.id} className="room-card">
              <h4>{room.name}</h4>
              <p>{room.description}</p>
              <p><strong>Price:</strong> ${room.price} / night</p>
              <button onClick={() => bookRoom(room.id)}>Book Now</button>
            </div>
          ))}
        </div>
      </section>

      <section className="bookings-section">
        <h3>My Bookings</h3>
        {bookings.length === 0 ? <p>No bookings yet.</p> : bookings.map(b => (
          <div key={b.bookingId} className="booking-card">
            <p><strong>Room:</strong> {b.roomName}</p>
            <p><strong>Check-in:</strong> {new Date(b.checkInDate).toLocaleDateString()}</p>
            <p><strong>Check-out:</strong> {new Date(b.checkOutDate).toLocaleDateString()}</p>
          </div>
        ))}
      </section>
    </div>
  );
};

export default UserDashboard;
