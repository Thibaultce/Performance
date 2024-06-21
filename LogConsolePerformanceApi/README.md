# Log Performance benchmark

Run performance tests of the Console & Azure Application Insights sinks of Serilog.

Test performed: 10 concurrent users for 30 seconds using k6.io.

```
k6 run script.js
```

## Results
| Console enabled | App Insights enabled | Throughput (req./sec) |   
|---|---|---|
| NO | NO | 65 162 |   
| YES | NO | 848 |  
| NO | YES | 20 647 |
| YES | YES | 844 |   