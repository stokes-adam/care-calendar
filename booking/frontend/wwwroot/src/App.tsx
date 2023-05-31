import { Route, Routes } from 'react-router-dom';
import './App.css';

import appRoutes, { AppRoute } from './routes';

function recursiveRoutes(routes: AppRoute[]): JSX.Element[] {
  return routes.map((route) => {
    if (route.children) {
      return (
        <Route key={route.path} path={route.path} element={route.element}>
          <Route key={route.path} index element={route.index} />
          {recursiveRoutes(route.children)}
        </Route>
      );
    }

    return <Route key={route.path} path={route.path} element={route.element} />;
  });
}

function App() {
  return (
    <div className="App">
      <Routes>
        {recursiveRoutes(appRoutes)}
      </Routes>
    </div>
  );
}

export default App;
