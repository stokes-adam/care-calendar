import { Breadcrumb, Calendar, Layout, theme } from "antd";
import { Content } from "antd/es/layout/layout";

function AppCalendar() {

    const {
        token: { colorBgContainer },
        } = theme.useToken();



    return (
        <>
        <Layout style={{ padding: '24px 0', background: colorBgContainer }}>
          <Content style={{ padding: '0 24px', minHeight: 280 }}>
            <Calendar />
          </Content>
        </Layout>
        </>
    );   
}

export default AppCalendar;