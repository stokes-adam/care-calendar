import { Outlet, useNavigate } from "react-router-dom";
import { Layout, Menu } from 'antd';
import type { MenuProps } from 'antd';

const { Header, Content, Footer, Sider } = Layout;

const mainMenuItems: MenuProps['items'] = [
  {
    key: '/',
    label: '<Firm Name>',
  },
  {
    key: '/clients',
    label: 'Clients',
  },
  {
    key: '/consultants',
    label: 'Consultants',
  },
  {
    key: '/rooms',
    label: 'Rooms',
  },
  {
    key: '/calendar',
    label: 'Calendar',
  }
];

function AppLayout() {
  const navigate = useNavigate();

  const navigateTo = (key: string) => {
    navigate(key);
  };

  return (
    <Layout style={{ maxHeight: '100vh' }}>
      <Header style={{ display: 'flex', alignItems: 'center' }}>
        <div className="demo-logo" />
        <Menu theme="dark" mode="horizontal" defaultSelectedKeys={['home']} items={mainMenuItems} onClick={(e) => navigateTo(e.key)}>
        </Menu>
      </Header>
      <Content style={{ padding: '0 50px' }}>
        <Outlet />

      </Content>
      <Footer style={{ textAlign: 'center' }}>Care Calendar Booking, Created by Adam Stokes</Footer>
    </Layout>
  )
}

export default AppLayout;