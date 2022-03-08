namespace TourPlanner.Common.Models
{

    public class Rootobject
    {
        public Route route { get; set; }
        public Info info { get; set; }
    }

    public class Route
    {
        public bool hasTollRoad { get; set; }
        public object[] computedWaypoints { get; set; }
        public float fuelUsed { get; set; }
        public bool hasUnpaved { get; set; }
        public bool hasHighway { get; set; }
        public int realTime { get; set; }
        public Boundingbox boundingBox { get; set; }
        public float distance { get; set; }
        public int time { get; set; }
        public int[] locationSequence { get; set; }
        public bool hasSeasonalClosure { get; set; }
        public string sessionId { get; set; }
        public Location[] locations { get; set; }
        public bool hasCountryCross { get; set; }
        public Leg[] legs { get; set; }
        public string formattedTime { get; set; }
        public Routeerror routeError { get; set; }
        public Options options { get; set; }
        public bool hasFerry { get; set; }
    }

    public class Boundingbox
    {
        public Ul ul { get; set; }
        public Lr lr { get; set; }
    }

    public class Ul
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    public class Lr
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    public class Routeerror
    {
        public string message { get; set; }
        public int errorCode { get; set; }
    }

    public class Options
    {
        public object[] mustAvoidLinkIds { get; set; }
        public int drivingStyle { get; set; }
        public bool countryBoundaryDisplay { get; set; }
        public int generalize { get; set; }
        public string narrativeType { get; set; }
        public string locale { get; set; }
        public bool avoidTimedConditions { get; set; }
        public bool destinationManeuverDisplay { get; set; }
        public bool enhancedNarrative { get; set; }
        public int filterZoneFactor { get; set; }
        public int timeType { get; set; }
        public int maxWalkingDistance { get; set; }
        public string routeType { get; set; }
        public int transferPenalty { get; set; }
        public bool stateBoundaryDisplay { get; set; }
        public int walkingSpeed { get; set; }
        public int maxLinkId { get; set; }
        public object[] arteryWeights { get; set; }
        public object[] tryAvoidLinkIds { get; set; }
        public string unit { get; set; }
        public int routeNumber { get; set; }
        public string shapeFormat { get; set; }
        public int maneuverPenalty { get; set; }
        public bool useTraffic { get; set; }
        public bool returnLinkDirections { get; set; }
        public object[] avoidTripIds { get; set; }
        public string manmaps { get; set; }
        public int highwayEfficiency { get; set; }
        public bool sideOfStreetDisplay { get; set; }
        public int cyclingRoadFactor { get; set; }
        public int urbanAvoidFactor { get; set; }
    }

    public class Location
    {
        public Latlng latLng { get; set; }
        public string adminArea4 { get; set; }
        public string adminArea5Type { get; set; }
        public string adminArea4Type { get; set; }
        public string adminArea5 { get; set; }
        public string street { get; set; }
        public string adminArea1 { get; set; }
        public string adminArea3 { get; set; }
        public string type { get; set; }
        public Displaylatlng displayLatLng { get; set; }
        public int linkId { get; set; }
        public string postalCode { get; set; }
        public string sideOfStreet { get; set; }
        public bool dragPoint { get; set; }
        public string adminArea1Type { get; set; }
        public string geocodeQuality { get; set; }
        public string geocodeQualityCode { get; set; }
        public string adminArea3Type { get; set; }
    }

    public class Latlng
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    public class Displaylatlng
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    public class Leg
    {
        public bool hasTollRoad { get; set; }
        public int index { get; set; }
        public object[][] roadGradeStrategy { get; set; }
        public bool hasHighway { get; set; }
        public bool hasUnpaved { get; set; }
        public float distance { get; set; }
        public int time { get; set; }
        public int origIndex { get; set; }
        public bool hasSeasonalClosure { get; set; }
        public string origNarrative { get; set; }
        public bool hasCountryCross { get; set; }
        public string formattedTime { get; set; }
        public string destNarrative { get; set; }
        public int destIndex { get; set; }
        public Maneuver[] maneuvers { get; set; }
        public bool hasFerry { get; set; }
    }

    public class Maneuver
    {
        public Sign[] signs { get; set; }
        public int index { get; set; }
        public object[] maneuverNotes { get; set; }
        public int direction { get; set; }
        public string narrative { get; set; }
        public string iconUrl { get; set; }
        public float distance { get; set; }
        public int time { get; set; }
        public object[] linkIds { get; set; }
        public string[] streets { get; set; }
        public int attributes { get; set; }
        public string transportMode { get; set; }
        public string formattedTime { get; set; }
        public string directionName { get; set; }
        public string mapUrl { get; set; }
        public Startpoint startPoint { get; set; }
        public int turnType { get; set; }
    }

    public class Startpoint
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    public class Sign
    {
        public string text { get; set; }
        public string extraText { get; set; }
        public int direction { get; set; }
        public int type { get; set; }
        public string url { get; set; }
    }

    public class Info
    {
        public Copyright copyright { get; set; }
        public int statuscode { get; set; }
        public object[] messages { get; set; }
    }

    public class Copyright
    {
        public string text { get; set; }
        public string imageUrl { get; set; }
        public string imageAltText { get; set; }
    }

    public class Class1
    {

    }
}