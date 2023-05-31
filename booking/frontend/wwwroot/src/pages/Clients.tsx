import { Table } from "antd";

const dataSource = [
    {
      key: '1',
      name: 'Mike',
      age: 32,
      address: '10 Downing Street',
    },
    {
      key: '2',
      name: 'John',
      age: 42,
      address: '10 Downing Street',
    },
  ];
  
  const columns = [
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Age',
      dataIndex: 'age',
      key: 'age',
    },
    {
      title: 'Address',
      dataIndex: 'address',
      key: 'address',
    },
  ];


function Clients() {
    return (
        <div>
            <h1>Clients</h1>
            <div style={{ display: 'flex', gap: '1em', justifyContent: 'center' }}>
                <button>Button element</button>
                <input type="submit" value="submit input" />
                <input type="button" value="button input" />

                <button className="button-primary">Button element</button>
                <input className="button-primary" type="submit" value="submit input" />
                <input className="button-primary" type="button" value="button input" />
            </div>
            <div style={{ width: '50%', margin: '0 auto' }}>
            <Table columns={columns} dataSource={dataSource} />
            </div>
        </div>
    )
}

export default Clients;