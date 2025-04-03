# Incremental Calculation

## How Microsoft Sustainability Manager Works for Incremental Calculation?
  
Microsoft Sustainability Manager (MSM) currently can run [emissions calculations automatically](https://learn.microsoft.com/industry/sustainability/calculate-calculation-profiles) for those activity records being imported using the Data Import functionality of MSM. To have this built-in automation to be triggered, users will need to select the below checkbox on the calculation profiles.

<img width="628" alt="image" src="https://github.com/user-attachments/assets/444f796a-17e1-48cd-9389-5dd76ae51171" />

## What Additional Scenarios This Accelerator Supports?

Currently, it is required to run calculations manually for scenarios where data is imported or updated without the Data Import Functionality and when you run the calculation profiles manually, it deletes the emissions records and re-create them. This also revokes any approval on the existing emission even if the underlying activity data or the calculation logic hasn't been changed.  

This accelerator is aiming to address these automation gaps for below scenarios:

### Scenario-1: Activity data is entered manually into system

### Scenario-2: Activity data is imported using Excel/CSV files using PowerApps Import capabilities

### Scenario-3: Activity data is imported using Excel/CSV files using PowerApps Import capabilities

### Scenario-3: Data Approval Management is enabled for Activity data and calculation needs to be run when they are approved

## Solution Overview


## Implementation Details

This accelerator has two approaches to run automation

### Approach-1: Mark records that will be the scope for incremental calculation

### Approach-2: Update existing calculation profiles with additional filter to run incrementally

## Deployment Steps 

