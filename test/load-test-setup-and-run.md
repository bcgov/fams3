# Load Test Setup And Run

## Setup

Instructions on k6 setup can be found [here](https://docs.k6.io/docs/installation).

## Run

### Instructions

1. Convert postman collection json file **./load-test.json** to k6 script.

        $ postman-to-k6 load-test.json -e environment.json -o load-test-script.js
        
        -- load-test.json - postman collection json file
        -- environment.json - postman environment json file
        -- load-test-script.js - Output k6 script
       
2. Run the k6 script **./load-test-script.js** to perform load test
    1. Method 1: **Simple**
        
            $ k6 run --vus 25 -i 3000 --out influxdb=http://localhost:8086/myk6db load-test-script.js
            
            --vus               - Number of Virtual users
            -i                  - Number of Iterations (A total of 3000 requests will be made where each vu will make 120 requests)
            influxdb            - Address of the influxdb to output the results of the load test (Optional).
            load-test-script.js - Name of the k6 file to be run.        
            
    2. Method 2: **Burst**
    
            $ k6 run --stage 6s:60,3s:60,6s:100,3s:100,6s:200,3s:200,6s:100,3s:0 --out influxdb=http://localhost:8086/myk6db load-test-script.js
            
            --stage - The number of stages the test should be run
                       duration:target
             
            --stage 6s:60,3s:60,6s:100,3s:100,6s:200,3s:200,6s:100,3s:0 can be understood as
            
            Stages: 
                   [{duration: "6s", target: 60}, For the first 6 seconds, ramp up 60 users
                    {duration: "3s", target: 60}, For the next 3 secs, stay flat at 60 users
                    {duration: "6s", target: 100}, For the next 6 seconds, reach 100 users
                    {duration: "3s", target: 100}, For the next 3 seconds, stay flat at 100 users
                    {duration: "6s", target: 200}, For the next 6 seconds, reach 200 users
                    {duration: "3s", target: 200}, For the next 3 seconds, stay flat at 200 users
                    {duration: "6s", target: 100}, For the next 6 seconds, ramp up down to 100 users
                    {duration: "3s", target: 0}, For the next 3 seconds, ramp up down to 0 users ]

More on k6 command line options can be found on [official-k6-documents-options](https://docs.k6.io/docs/options).
       