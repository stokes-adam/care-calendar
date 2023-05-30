import Table from "../components/Table";

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
            <Table />
            </div>
        </div>
    )
}

export default Clients;