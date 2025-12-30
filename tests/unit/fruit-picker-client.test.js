const FruitPickerClient = require('../../src/services/fruit-picker-client');

describe('FruitPickerClient', () => {
  let client;

  beforeEach(() => {
    client = new FruitPickerClient();
  });

  test('should create client with default configuration', () => {
    expect(client.baseUrl).toBeDefined();
    expect(client.apiKey).toBeDefined();
  });

  test('should have pickFruit method', () => {
    expect(typeof client.pickFruit).toBe('function');
  });

  test('should have getJobStatus method', () => {
    expect(typeof client.getJobStatus).toBe('function');
  });
});
