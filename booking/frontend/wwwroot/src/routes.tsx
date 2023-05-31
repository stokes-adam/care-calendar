import AppLayout from "./components/AppLayout";
import AppCalendar from "./pages/AppCalendar";
import Clients from "./pages/Clients";
import Consultants from "./pages/Consultants";
import Home from "./pages/Home";
import NotFound from "./pages/NotFound";
import Rooms from "./pages/Rooms";

export type AppRoute = {
  path: string;
  element: JSX.Element;
  index?: JSX.Element;
  children?: AppRoute[];
};

const appRoutes: AppRoute[] = [
  {
    path: '/',
    element: <AppLayout />,
    index: <Home />,
    children: [
      {
        path: 'clients',
        element: <Clients />,
      },
      {
        path: 'consultants',
        element: <Consultants />,
      },
      {
        path: 'rooms',
        element: <Rooms />,
      },
      {
        path: 'calendar',
        element: <AppCalendar />,
      },
      {
        path: '*',
        element: <NotFound />,
      },
    ]
  },
];

export default appRoutes;