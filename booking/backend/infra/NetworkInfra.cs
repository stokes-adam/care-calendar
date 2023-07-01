using Pulumi;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;

namespace infra;

public class NetworkInfra
{
    private Vpc Vpc { get; }
    private Subnet Subnet1 { get; }
    private Subnet Subnet2 { get; }
    private SecurityGroup SecurityGroup { get; }
    
    public NetworkInfra(CustomResourceOptions customResourceOptions)
    {
        Vpc = new Vpc("vpc", new VpcArgs
        {
            CidrBlock = "10.0.0.0/16",
            EnableDnsHostnames = true,
            EnableDnsSupport = true,
        }, customResourceOptions);

        var internetGateway = new InternetGateway("internetGateway", new InternetGatewayArgs
        {
        }, customResourceOptions);

        var vpcInternetGatewayAttachment = new InternetGatewayAttachment("vpcInternetGatewayAttachment", new()
        {
            VpcId = Vpc.Id,
            InternetGatewayId = internetGateway.Id,
        }, customResourceOptions);

        var routeTable = new RouteTable("routeTable", new RouteTableArgs
        {
            VpcId = Vpc.Id,
            Routes =
            {
                new RouteTableRouteArgs
                {
                    CidrBlock = "0.0.0.0/0",
                    GatewayId = internetGateway.Id,
                }
            },
        }, customResourceOptions);

        Subnet1 = new Subnet("subnet1", new SubnetArgs
        {
            VpcId = Vpc.Id,
            CidrBlock = "10.0.1.0/24",
        }, customResourceOptions);

        Subnet2 = new Subnet("subnet2", new SubnetArgs
        {
            VpcId = Vpc.Id,
            CidrBlock = "10.0.2.0/24",
        }, customResourceOptions);

        var routeTableAssociation1 = new RouteTableAssociation("routeTableAssociation1", new RouteTableAssociationArgs
        {
            SubnetId = Subnet1.Id,
            RouteTableId = routeTable.Id,
        }, customResourceOptions);

        var routeTableAssociation2 = new RouteTableAssociation("routeTableAssociation2", new RouteTableAssociationArgs
        {
            SubnetId = Subnet2.Id,
            RouteTableId = routeTable.Id,
        }, customResourceOptions);

        SecurityGroup = new SecurityGroup("securityGroup", new SecurityGroupArgs
        {
            VpcId = Vpc.Id,
            Ingress =
            {
                new SecurityGroupIngressArgs
                {
                    Protocol = "tcp",
                    FromPort = 80,
                    ToPort = 80,
                    CidrBlocks = { "0.0.0.0/0" },
                }
            },
            Egress =
            {
                new SecurityGroupEgressArgs
                {
                    Protocol = "-1",
                    FromPort = 0,
                    ToPort = 0,
                    CidrBlocks = { "0.0.0.0/0" }
                }
            }
        }, customResourceOptions);
    }
    
    public Output<string> VpcId => Vpc.Id;
    public Output<string> Subnet1Id => Subnet1.Id;
    public Output<string> Subnet2Id => Subnet2.Id;
    public Output<string> SecurityGroupId => SecurityGroup.Id;
}