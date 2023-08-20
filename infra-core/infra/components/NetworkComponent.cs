using System.Linq;
using Pulumi;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;

namespace components;

public class NetworkComponent
{
    private readonly CustomResourceOptions _customResourceOptions;
    private RouteTable RouteTable { get; }

    public NetworkComponent(CustomResourceOptions customResourceOptions)
    {
        _customResourceOptions = customResourceOptions;
        
        Vpc = new Vpc("vpc", new VpcArgs
        {
            CidrBlock = "10.0.0.0/16",
            EnableDnsHostnames = true,
            EnableDnsSupport = true,
        }, customResourceOptions);

        var internetGateway = new InternetGateway("internetGateway", new(), customResourceOptions);

        var vpcInternetGatewayAttachment = new InternetGatewayAttachment("vpcInternetGatewayAttachment", new()
        {
            VpcId = Vpc.Id,
            InternetGatewayId = internetGateway.Id,
        }, customResourceOptions);

        RouteTable = new RouteTable("routeTable", new RouteTableArgs
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
        
        SubnetIds = new InputList<string>();
        CreateSubnet("10.0.3.0/24", "us-east-1a");
        CreateSubnet("10.0.4.0/24", "us-east-1b");

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
                },
                new SecurityGroupIngressArgs
                {
                    Protocol = "tcp",
                    FromPort = 5057,
                    ToPort = 5057,
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
    
    private void CreateSubnet(string cidrBlock, string availabilityZone)
    {
        var num = SubnetIds.Apply(subnets => subnets.Length + 1);
        
        var subnet = new Subnet($"subnet{num}", new SubnetArgs
        {
            VpcId = Vpc.Id,
            CidrBlock = cidrBlock,
            AvailabilityZone = availabilityZone
        }, _customResourceOptions);
        
        var routeTableAssociation = new RouteTableAssociation($"routeTableAssociation{num}", new RouteTableAssociationArgs
        {
            SubnetId = subnet.Id,
            RouteTableId = RouteTable.Id,
        }, _customResourceOptions);
        
        SubnetIds.Add(subnet.Id);
    }
    
    public Vpc Vpc { get; }
    public InputList<string> SubnetIds { get; }
    public SecurityGroup SecurityGroup { get; }
}