import React from "react";
import { Breadcrumb, Layout, Menu, MenuProps, theme } from "antd";
import Sider from "antd/es/layout/Sider";
import { Content } from "antd/es/layout/layout";
import { LaptopOutlined, NotificationOutlined, UserOutlined } from '@ant-design/icons';


const items2: MenuProps['items'] = [UserOutlined, LaptopOutlined, NotificationOutlined].map(
  (icon, index) => {
    const key = String(index + 1);

    return {
      key: `sub${key}`,
      icon: React.createElement(icon),
      label: `subnav ${key}`,

      children: new Array(4).fill(null).map((_, j) => {
        const subKey = index * 4 + j + 1;
        return {
          key: subKey,
          label: `option${subKey}`,
        };
      }),
    };
  },
);

function Consultants() {

  const {
    token: { colorBgContainer },
  } = theme.useToken();

    return (
        <>
       <Breadcrumb style={{ margin: '16px 0' }} items={[{ title: "Home" }, { title: "List" }, { title: "Thing" }]} />
        <Layout style={{ padding: '24px 0', background: colorBgContainer }}>
          <Sider style={{ background: colorBgContainer }} width={200}>
            <Menu
              mode="inline"
              defaultSelectedKeys={['1']}
              defaultOpenKeys={['sub1']}
              style={{ height: '100%' }}
              items={items2}
            />
          </Sider>
          <Content style={{ padding: '0 24px', minHeight: 280 }}>Content</Content>
        </Layout>
        </>
    );
}

export default Consultants;