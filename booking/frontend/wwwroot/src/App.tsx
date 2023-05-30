import { Route, Routes } from 'react-router-dom';
import './App.css';

// pages
import Home from './pages/Home';
import Clients from './pages/Clients';
import Consultants from './pages/Consultants';
import Rooms from './pages/Rooms';

// components
import Layout from './components/Layout';
import NotFound from './pages/NotFound';

function App() {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Home />} />
          <Route path="clients" element={<Clients />} />
          <Route path="consultants" element={<Consultants />} />
          <Route path="rooms" element={<Rooms />} />
          <Route path="*" element={<NotFound />} />
        </Route>
      </Routes>
    </div>
  );
}

export default App;
