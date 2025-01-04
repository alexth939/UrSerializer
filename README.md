# UrSerializer
 Lightweight solution for custom serialization

## Issues and Workarounds

### ISerializationCallbackReceiver.OnBeforeSerialize Inoked repeatedly
The technique that we use to hook into unity's serialization process is utilizing **UnityEngine.ISerializationCallbackReceiver.OnBeforeSerialize** callback.

#### Issue
Unexpected behaviour revealed:
- Implement this interface on scriptable object.
- Add Debug.Log("buya!") in OnBeforeSerialize();
- Add some serialized field, so it will be shown in inspector when selected.
- Create instance of this scriptable object.
- In project window, select the instance to see his inspector.
- Check the console, it will log "buya" repeatedly, every frame.

#### Solution
Find out who invokes OnBeforeSerialize() every frame, and abort if u find him on stack trace.
[Discussion](https://discussions.unity.com/t/onbeforeserialize-is-getting-called-rapidly/115546)
