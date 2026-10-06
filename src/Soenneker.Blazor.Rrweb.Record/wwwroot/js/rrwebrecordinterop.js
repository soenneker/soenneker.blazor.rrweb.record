// rrweb 2.1.7. Only one recorder can own rrweb's document-wide observers.
let activeRecorder = null;

export function createInterop() {
    const owner = {};
    let events = [];
    let stopRecording = null;
    let disposed = false;

    function ensureActive() {
        if (disposed) throw new Error("The recorder has been disposed.");
        if (activeRecorder !== owner || !stopRecording) throw new Error("Recording has not started.");
    }

    function stop() {
        if (!stopRecording) return;
        const stopFn = stopRecording;
        stopRecording = null;
        try { stopFn(); }
        finally { if (activeRecorder === owner) activeRecorder = null; }
    }

    return {
        start(optionsJson) {
            if (disposed) throw new Error("The recorder has been disposed.");
            if (activeRecorder !== null) throw new Error("An rrweb recorder is already active in this document.");
            const record = globalThis.rrwebRecord?.record;
            if (typeof record !== "function") throw new Error("rrweb recording resources are not loaded.");
            const options = JSON.parse(optionsJson);
            const previousEvents = events;
            events = [];
            activeRecorder = owner;
            try {
                stopRecording = record({ ...options, emit: event => events.push(event) });
                if (typeof stopRecording !== "function") throw new Error("rrweb could not start recording.");
            } catch (error) {
                activeRecorder = null;
                stopRecording = null;
                events = previousEvents;
                throw error;
            }
        },
        stop,
        isRecording() { return stopRecording !== null && activeRecorder === owner; },
        getEvents(clear = false) {
            if (disposed) throw new Error("The recorder has been disposed.");
            const json = JSON.stringify(events);
            if (clear) events = [];
            return json;
        },
        addCustomEvent(tag, payloadJson) {
            ensureActive();
            globalThis.rrwebRecord.record.addCustomEvent(tag, JSON.parse(payloadJson));
        },
        takeFullSnapshot(isCheckout = false) {
            ensureActive();
            globalThis.rrwebRecord.record.takeFullSnapshot(isCheckout);
        },
        dispose() {
            if (disposed) return;
            try { stop(); }
            finally { events = []; disposed = true; }
        }
    };
}
