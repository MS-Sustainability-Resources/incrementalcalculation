# Incremental Calculation

## How Microsoft Sustainability Manager Works for Incremental Calculation?
  
Microsoft Sustainability Manager (MSM) currently can run [emissions calculations automatically](https://learn.microsoft.com/industry/sustainability/calculate-calculation-profiles) for those activity records being imported using the Data Import functionality of MSM. To have this built-in automation to be triggered, users will need to select the below checkbox on the calculation profiles.

<img width="628" alt="image" src="https://github.com/user-attachments/assets/444f796a-17e1-48cd-9389-5dd76ae51171" />

## What Additional Scenarios This Accelerator Supports?

Currently, it is required to run calculations manually for scenarios where data is imported or updated without the Data Import Functionality and when you run the calculation profiles manually, it deletes the emissions records and re-create them. This also revokes any approval on the existing emission even if the underlying activity data or the calculation logic hasn't been changed.  

This accelerator is aiming to address these automation gaps for below scenarios:

### Scenario-1: Activity data is entered manually into system

### Scenario-2: Activity data is imported using Excel/CSV files using PowerApps Import capabilities

### Scenario-3: Data Approval Management is enabled for Activity data and calculation needs to be run when they are approved

## Solution Overview


## Implementation Details

This accelerator has two approaches to run automation

### Approach-1: Mark records that will be the scope for incremental calculation

The "Scheduled - Run Incremental Calculation" power automate will run at a schedule to run calculation profiles that are auto-run enabled with the changed records. Below is the filtering criteria to choose which tables to operate on.

<img width="489" alt="image" src="https://github.com/user-attachments/assets/f3ed667c-bed8-4bf6-a2b6-36a73587ff7b" />

To achieve incremental calculation, calculation profiles will need to be executed with an additional parameter as connection refresh id as shown below

<img width="506" alt="image" src="https://github.com/user-attachments/assets/d6b98f58-6dd8-4c18-8374-6598e97bfb52" />

All new approved and new manual records will be updated with a system generated connection refresh record so that all of them can be marked for execution.

Connection refresh record will be created with the data import record generated from the system connector.

<img width="485" alt="image" src="https://github.com/user-attachments/assets/872a7edf-19d8-4031-b0c8-3fa7e5ddbf1d" />

This connection refresh record will then be used to update the records connection refresh column. This column name is not standard, therefore added an if clause to treat specific tables differently.

<img width="1040" alt="image" src="https://github.com/user-attachments/assets/74c5c164-ca9a-45a9-8aea-dc7192e07854" />

As approved records are closed for update, we will need to temporarily disable data approval for those data approval enabled entities

<img width="1084" alt="image" src="https://github.com/user-attachments/assets/56c6cdbf-7860-4675-8175-fe11239ab364" />

and later, we will need to enable it back.

<img width="1044" alt="image" src="https://github.com/user-attachments/assets/38418d84-f53b-4ec4-80f3-e8949c6cb8e1" />

Incremental logic requires a data to iterate from. It uses below dates with the sequence

- Last successful run of the "Scheduled - Run Incremental Calculation" flow
- Modifiedon date field of the sustainability data definition record for the relevant table  

### Approach-2: Update existing calculation profiles with additional filter to run incrementally

This approach appends a filter criteria for the calculation profiles. This approach doesnt require to update records or update approval setting of the entity temporarily. The "Run profiles for an activity type with incremental filter" power automate runs the flow for the given table. The flow gets the last successful execution of that calculation profile and adds a filter clause to the profile to aim for records that are added / updated after that date. Below are the steps in the flow that accomplishes the filter update

<img width="487" alt="image" src="https://github.com/user-attachments/assets/8d015f06-58b6-47f6-afdd-14b1ad69c91c" />

