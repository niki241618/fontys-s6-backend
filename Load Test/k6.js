import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Define an error rate metric
export let errorRate = new Rate('errors');

// Define the test options
export let options = {
    stages: [
        { duration: '2m', target: 100 }, // ramp-up to 100 users over 2 minutes
        { duration: '5m', target: 100 }, // maintain 100 users for 5 minutes
        { duration: '2m', target: 0 },   // ramp-down to 0 users over 2 minutes
    ],
    thresholds: {
        'http_req_duration': ['p(95)<300'], // 95% of requests must complete below 200ms
        'errors': ['rate<0.01'],            // <1% errors
    },
};

export default function () {
    // Define the headers
    let headers = {
        'Content-Type': 'application/json',
    };

    // GET /api/audiobooks
    let getAllRes = http.get('http://localhost/api/audiobooks', { headers: headers });
    check(getAllRes, {
        'GET /api/audiobooks is status 200': (r) => r.status === 200,
    }) || errorRate.add(1); // Increment error rate if check fails

    // GET /api/audiobooks/1
    let getOneRes = http.get('http://localhost/api/audiobooks/1', { headers: headers });
    check(getOneRes, {
        'GET /api/audiobooks/1 is status 200': (r) => r.status === 200,
    }) || errorRate.add(1); // Increment error rate if check fails

    // Sleep for 1 second between iterations
    sleep(1);
}
