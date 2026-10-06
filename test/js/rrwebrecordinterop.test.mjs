import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';

const source = await readFile(new URL('../../src/Soenneker.Blazor.Rrweb.Record/wwwroot/js/rrwebrecordinterop.js', import.meta.url), 'utf8');
const { createInterop } = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);

function setup() {
    let emit;
    let stops = 0;
    const record = options => { emit = options.emit; emit({ type: 2, data: {}, timestamp: 1 }); return () => stops++; };
    record.addCustomEvent = (tag, payload) => emit({ type: 5, data: { tag, payload }, timestamp: 2 });
    record.takeFullSnapshot = () => emit({ type: 2, data: {}, timestamp: 3 });
    globalThis.rrwebRecord = { record };
    return { emit: event => emit(event), stops: () => stops };
}

test('drains in order, preserves custom events, and retains the buffer on stop', () => {
    const fake = setup();
    const recorder = createInterop();
    try {
        recorder.start('{}');
        recorder.addCustomEvent('test', '{"value":1}');
        assert.deepEqual(JSON.parse(recorder.getEvents(true)).map(e => e.type), [2, 5]);
        assert.deepEqual(JSON.parse(recorder.getEvents()), []);
        recorder.takeFullSnapshot();
        recorder.stop();
        recorder.stop();
        assert.equal(fake.stops(), 1);
        assert.equal(JSON.parse(recorder.getEvents()).length, 1);
        assert.equal(recorder.isRecording(), false);
    } finally { recorder.dispose(); }
});

test('another instance cannot steal the document recorder or stop its owner', () => {
    setup();
    const first = createInterop(), second = createInterop();
    try {
        first.start('{}');
        assert.throws(() => second.start('{}'), /already active/);
        second.stop();
        assert.equal(first.isRecording(), true);
        first.dispose();
        second.start('{}');
        assert.equal(second.isRecording(), true);
    } finally { first.dispose(); second.dispose(); }
});

test('a failed restart preserves the previous recording and releases ownership', () => {
    setup();
    const recorder = createInterop();
    recorder.start('{}');
    recorder.stop();
    const saved = recorder.getEvents();
    globalThis.rrwebRecord.record = () => undefined;
    assert.throws(() => recorder.start('{}'), /could not start/);
    assert.equal(recorder.getEvents(), saved);
    setup();
    recorder.start('{}');
    recorder.dispose();
    assert.throws(() => recorder.start('{}'), /disposed/);
});

test('custom events and snapshots require an active recording', () => {
    setup();
    const recorder = createInterop();
    assert.throws(() => recorder.addCustomEvent('test', '{}'), /not started/);
    assert.throws(() => recorder.takeFullSnapshot(), /not started/);
    recorder.dispose();
});
