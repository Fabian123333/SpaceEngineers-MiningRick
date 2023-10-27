List<IMyShipDrill> drills = new List<IMyShipDrill>();

IMyBlockGroup pistonDownGroup;
List<IMyPistonBase> pistonDown = new List<IMyPistonBase>();

IMyBlockGroup pistonUpGroup;
List<IMyPistonBase> pistonUp = new List<IMyPistonBase>();

IMyBlockGroup pistonBackGroup;
List<IMyPistonBase> pistonBack = new List<IMyPistonBase>();

IMyBlockGroup pistonFrontGroup;
List<IMyPistonBase> pistonFront = new List<IMyPistonBase>();

IMyBlockGroup pistonLeftGroup;
List<IMyPistonBase> pistonLeft = new List<IMyPistonBase>();

IMyBlockGroup pistonRightGroup;
List<IMyPistonBase> pistonRight = new List<IMyPistonBase>();

const float SlowMovementSpeed = 0.25f;
const float FastMovementSpeed = 4f;

float InitialDepth = 10;
float layerDepth = 2.2f;
float rowWidth = 3.4f;

Vector3 positionOffset = new Vector3(10, 10, 0);

int currentLayer = 0;
int currentRow = 0;
float currentY = 0;
bool lastRow = false;
float DepthTarget = 0;

public enum Axis{
    X,
    Y,
    Z
}

public enum State {
    None,
    Prepare,
    LowerDrill,
    MineRow,
    ChangeRow,
}

State CurrentState = State.None;

public Program()
{ 
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    Load(Storage);
    LoadPistons();
    LoadDrill();
    Echo("Initialize Mining Grid");
}

private void SetMovementSpeed(float velocity){
    foreach(IMyPistonBase piston in pistonUp){
        piston.Velocity = velocity;
    }

    foreach(IMyPistonBase piston in pistonDown){
        piston.Velocity = velocity;
    }

    foreach(IMyPistonBase piston in pistonLeft){
        piston.Velocity = velocity;
    }

    foreach(IMyPistonBase piston in pistonRight){
        piston.Velocity = velocity;
    }

    foreach(IMyPistonBase piston in pistonFront){
        piston.Velocity = velocity;
    }

    foreach(IMyPistonBase piston in pistonBack){
        piston.Velocity = velocity;
    }
}

private void SetPistonState(bool enabled){
    foreach(IMyPistonBase piston in pistonUp){
        piston.Enabled = enabled;
    }

    foreach(IMyPistonBase piston in pistonDown){
        piston.Enabled = enabled;
    }

    foreach(IMyPistonBase piston in pistonLeft){
        piston.Enabled = enabled;
    }

    foreach(IMyPistonBase piston in pistonRight){
        piston.Enabled = enabled;
    }

    foreach(IMyPistonBase piston in pistonFront){
        piston.Enabled = enabled;
    }

    foreach(IMyPistonBase piston in pistonBack){
        piston.Enabled = enabled;
    }
}

private void CenterHome(){
    SetMovementSpeed(FastMovementSpeed);
    SetPistonState(true);

    CenterPistonsOnLayer();

    foreach(IMyPistonBase piston in pistonUp){
        piston.Extend();
    }

    foreach(IMyPistonBase piston in pistonDown){
        piston.Retract();
    }
}

private void CenterPistonsOnLayer(){
    SetMovementSpeed(FastMovementSpeed);

    foreach(IMyPistonBase piston in pistonLeft){
        piston.Retract();
    }

    foreach(IMyPistonBase piston in pistonBack){
        piston.Retract();
    }

    foreach(IMyPistonBase piston in pistonRight){
        piston.Extend();
    }

    foreach(IMyPistonBase piston in pistonFront){
        piston.Extend();
    }
}

private void LoadDrill(){
    // Für Drills
    IMyBlockGroup drillGroup = GridTerminalSystem.GetBlockGroupWithName("drill") as IMyBlockGroup;
    if (drillGroup != null) 
    {
        drillGroup.GetBlocksOfType<IMyShipDrill>(drills);
    }
}

private void SetDrillState(bool enabled){
    foreach(IMyShipDrill drill in drills){
        drill.Enabled = enabled;
    }
}

private Vector3 GetPosition(){
    float x = 0;
    float y = 0;
    float z = 0;

    foreach(IMyPistonBase piston in pistonUp){
        z -= (piston.CurrentPosition - piston.MaxLimit);
    }

    foreach(IMyPistonBase piston in pistonDown){
        z += (piston.CurrentPosition - piston.MinLimit);
    }

    foreach(IMyPistonBase piston in pistonBack){
        y -= (piston.CurrentPosition);
    }

    foreach(IMyPistonBase piston in pistonFront){
        y += (piston.CurrentPosition);
    }

    foreach(IMyPistonBase piston in pistonRight){
        x -= (piston.CurrentPosition);
    }

    foreach(IMyPistonBase piston in pistonLeft){
        x += (piston.CurrentPosition);
    }
    
    return new Vector3(x, -y, z) + positionOffset;
}

private bool ExtendAxis(Axis axis) {
    switch(axis){
        case Axis.X:
            foreach(IMyPistonBase piston in pistonRight){
                if(piston.Status != PistonStatus.Retracted){
                    piston.Retract();
                    piston.Enabled = true;
                    return true;
                }
            }

            foreach(IMyPistonBase piston in pistonLeft){
                if(piston.Status != PistonStatus.Extended){
                    piston.Extend();
                    piston.Enabled = true;
                    return true;
                }
            }

            break;

        case Axis.Y:
            foreach(IMyPistonBase piston in pistonBack){
                if(piston.Status != PistonStatus.Extended){
                    piston.Extend();
                    piston.Enabled = true;
                    return true;
                }
            }

            foreach(IMyPistonBase piston in pistonFront){
                if(piston.Status != PistonStatus.Retracted){
                    piston.Retract();
                    piston.Enabled = true;
                    return true;
                }
            }

            break;

        case Axis.Z:
            foreach(IMyPistonBase piston in pistonUp){
                if(piston.Status != PistonStatus.Retracted){
                    piston.Retract();
                    piston.Enabled = true;
                    return true;
                }
            }

            foreach(IMyPistonBase piston in pistonDown){
                if(piston.Status != PistonStatus.Extended){
                    piston.Extend();
                    piston.Enabled = true;
                    return true;
                }
            }

            break;

        default:
            Echo("not implemented");
            break;
    }
    
    return false;
}

private bool RetractAxis(Axis axis) {
    Echo("RetractAxis call");
    switch(axis){
        case Axis.X:
            foreach(IMyPistonBase piston in pistonLeft){
                if(piston.Status != PistonStatus.Retracted){
                    piston.Retract();
                    piston.Enabled = true;
                    return true;
                }
            }

            foreach(IMyPistonBase piston in pistonRight){
                if(piston.Status != PistonStatus.Extended){
                    piston.Extend();
                    piston.Enabled = true;
                    return true;
                }
            }

            break;

        case Axis.Y:
            foreach(IMyPistonBase piston in pistonFront){
                if(piston.Status != PistonStatus.Extended){
                    piston.Extend();
                    piston.Enabled = true;
                    return true;
                }
            }

            foreach(IMyPistonBase piston in pistonBack){
                if(piston.Status != PistonStatus.Retracted){
                    piston.Retract();
                    piston.Enabled = true;
                    return true;
                }
            }

            break;

        case Axis.Z:
            foreach(IMyPistonBase piston in pistonDown){
                if(piston.Status != PistonStatus.Retracted){
                    piston.Retract();
                    piston.Enabled = true;
                    return true;
                }
            }

            foreach(IMyPistonBase piston in pistonUp){
                if(piston.Status != PistonStatus.Extended){
                    piston.Extend();
                    piston.Enabled = true;
                    return true;
                }
            }

            break;

        default:
            Echo("not implemented");
            break;
    }
    
    return false;
}

private bool PistonIsMoving(Axis axis){
    switch(axis){
        case Axis.X:
            foreach(IMyPistonBase piston in pistonRight){
                if(piston.Enabled && (piston.Status != PistonStatus.Stopped && piston.Status != PistonStatus.Extended && piston.Status != PistonStatus.Retracted))
                    return true;
            }

            foreach(IMyPistonBase piston in pistonLeft){
                if(piston.Enabled && (piston.Status != PistonStatus.Stopped && piston.Status != PistonStatus.Extended && piston.Status != PistonStatus.Retracted))
                    return true;
            }

            break;

        case Axis.Y:
            foreach(IMyPistonBase piston in pistonBack){
                if(piston.Enabled & (piston.Status != PistonStatus.Stopped && piston.Status != PistonStatus.Extended && piston.Status != PistonStatus.Retracted))
                    return true;
            }
            
            foreach(IMyPistonBase piston in pistonFront){
                if(piston.Enabled && (piston.Status != PistonStatus.Stopped && piston.Status != PistonStatus.Extended && piston.Status != PistonStatus.Retracted))
                    return true;
            }

            break;

        case Axis.Z:
            foreach(IMyPistonBase piston in pistonUp){
                if(piston.Enabled && (piston.Status != PistonStatus.Stopped && piston.Status != PistonStatus.Extended && piston.Status != PistonStatus.Retracted))
                    return true;
            }
            
            foreach(IMyPistonBase piston in pistonDown){
                if(piston.Enabled && (piston.Status != PistonStatus.Stopped && piston.Status != PistonStatus.Extended && piston.Status != PistonStatus.Retracted))
                    return true;
            }

            break;

        default:
            Echo("error axis not defined");
            break;
    }

    return false;
}

private void LoadPistons(){
    // Für Piston Down
    pistonDownGroup = GridTerminalSystem.GetBlockGroupWithName("piston-grid-down") as IMyBlockGroup;
    if (pistonDownGroup != null) 
    {
        pistonDownGroup.GetBlocksOfType<IMyPistonBase>(pistonDown);
    }

    // Für Piston Up
    pistonUpGroup = GridTerminalSystem.GetBlockGroupWithName("piston-grid-up") as IMyBlockGroup;
    if (pistonUpGroup != null) 
    {
        pistonUpGroup.GetBlocksOfType<IMyPistonBase>(pistonUp);
    }

    // Für Piston Back
    pistonBackGroup = GridTerminalSystem.GetBlockGroupWithName("piston-grid-back") as IMyBlockGroup;
    if (pistonBackGroup != null) 
    {
        pistonBackGroup.GetBlocksOfType<IMyPistonBase>(pistonBack);
    }

    // Für Piston Front
    pistonFrontGroup = GridTerminalSystem.GetBlockGroupWithName("piston-grid-front") as IMyBlockGroup;
    if (pistonFrontGroup != null) 
    {
        pistonFrontGroup.GetBlocksOfType<IMyPistonBase>(pistonFront);
    }

    // Für Piston Left
    pistonLeftGroup = GridTerminalSystem.GetBlockGroupWithName("piston-grid-left") as IMyBlockGroup;
    if (pistonLeftGroup != null) 
    {
        pistonLeftGroup.GetBlocksOfType<IMyPistonBase>(pistonLeft);
    }

    // Für Piston Right
    pistonRightGroup = GridTerminalSystem.GetBlockGroupWithName("piston-grid-right") as IMyBlockGroup;
    if (pistonRightGroup != null) 
    {
        pistonRightGroup.GetBlocksOfType<IMyPistonBase>(pistonRight);
    }
}

public void Save(){
    Storage = string.Join(";",
        currentLayer.ToString(),
        currentRow.ToString(),
        currentY.ToString(),
        lastRow.ToString(),
        DepthTarget.ToString(),
        CurrentState.ToString()
    );
}

public void Load(string args){
    string[] storedData = args.Split(';');

    if(storedData.Length <= 5)
        return;

    int.TryParse(storedData[0], out currentLayer);
    int.TryParse(storedData[1], out currentRow);

    if(storedData[2] != "0"){
        float.TryParse(storedData[2], out currentY);
    }

    bool.TryParse(storedData[3], out lastRow);
    Enum.TryParse<State>(storedData[5], out CurrentState);
    float.TryParse(storedData[4], out DepthTarget);
}

public void Main(string argument, UpdateType updateType)
{   
    switch(CurrentState){
        case State.None:
            Echo("start auto miner");

            CurrentState = State.Prepare;
            CenterHome();
    	    SetDrillState(false);
            break;
        case State.Prepare:
            Echo("wait for miner to center, current position: " + GetPosition().ToString());

            if(GetPosition() == new Vector3(0,0,0)){
                SetMovementSpeed(SlowMovementSpeed);
                Echo("reached home, lowering drill");
                SetDrillState(true);
                SetPistonState(false);

                DepthTarget = InitialDepth;
                CurrentState = State.LowerDrill;
            }
            break;
        case State.LowerDrill:
            if(PistonIsMoving(Axis.Z)){
                if(GetPosition().Z >= DepthTarget){
                    Echo("reached new digging depth");
                    SetPistonState(false);
                    CurrentState = State.MineRow;
                    currentRow = 1;
                } else {
                    Echo("waiting for drill to lower, remaining: " + (DepthTarget - GetPosition().Z).ToString() + "m");
                }
            }
            else{
                ExtendAxis(Axis.Z);
            }

            break;
        case State.MineRow:
            if(!PistonIsMoving(Axis.X)){
                if(currentRow % 2 == 0)
                {
                    bool isExtending = false;
                    if(currentLayer % 2 == 0)
                        isExtending = ExtendAxis(Axis.X);
                    else
                        isExtending = RetractAxis(Axis.X);

                    if(!isExtending){
                        Echo("reached end of row");
                        currentY = GetPosition().Y;
                        currentRow++;
                        SetPistonState(false);
                        CurrentState = State.ChangeRow;
                    }
                }
                else
                {
                    bool isExtending = false;
                    if(currentLayer % 2 == 0)
                        isExtending = RetractAxis(Axis.X);
                    else
                        isExtending = ExtendAxis(Axis.X);

                    if(!isExtending){
                        Echo("reached end of row");
                        currentRow++;
                        currentY = GetPosition().Y;
                        SetPistonState(false);
                        CurrentState = State.ChangeRow;
                    }
                }
            } else {
                Echo("mine layer " + currentLayer.ToString() + " row " + currentRow.ToString());
            }

            break;

        case State.ChangeRow:
            Echo("finished layer change row, position: " + GetPosition().ToString());
            if((currentLayer % 2 == 0 && GetPosition().Y <= currentY - rowWidth) || ( currentLayer % 2 != 0 && GetPosition().Y >= currentY + rowWidth)){
                        SetPistonState(false);
                        Echo("finished change row, width reached");
                        CurrentState = State.MineRow;
                        return;
            }

            if(!PistonIsMoving(Axis.Y)){
                if(currentLayer % 2 == 0){
                    if(!RetractAxis(Axis.Y)){
                        SetPistonState(false);
                        if(lastRow)
                        {
                            currentLayer++;
                            DepthTarget += layerDepth;
                            Echo("finished layer, dig deeper");
                            CurrentState = State.LowerDrill;
                            lastRow = false;
                            return;
                        }
                        Echo("finished change row, end reached");
                        lastRow = true;
                        CurrentState = State.MineRow;
                    }
                } else {
                    if(!ExtendAxis(Axis.Y)){
                        SetPistonState(false);
                        if(lastRow)
                        {
                            currentLayer++;
                            DepthTarget += layerDepth;
                            Echo("finished layer, dig deeper");
                            CurrentState = State.LowerDrill;   
                            lastRow = false; 
                            return;
                        }
                        Echo("finished change row, end reached");
                        lastRow = true;
                        CurrentState = State.MineRow;
                    }
                }
            }
            break;

        default:
            Echo("undefined state");
            break;
    }
}
used
