# inmation and Azure Machine Learning Studio

## Getting your data has never been easier

Azure Machine Learning Studio supports Python and R script execution, which enables the user to input and output the data from and to the system:inmation, by creating inmation http Python client for example. Not to mention - easy data transformation, using build in models in either of the supported languages in between. Large or reusable scripts can also be stored in the zip files. Zip files can be imported and saved as datasets in the Azure ML Studio.
\
[Here](./scripts/inmationhttpclient.py) is an example code in Python with a few basic functions for accessing data stored in DataStudio. This script can be saved as zip file and uploaded from the local computer as dataset to Azure ML Studio.

## Invoking function from the Zip file

Functions from the Zip file can be invoked by adding "Execute Python Script" tile. Example code for invoking the ReadHistoricalData
endpoint is [here](./scripts/readhistoricaldata.py). In the Python script file, fill in the DataStudio item paths, which historical data should be read, start and end time and the intervals. Connect the zip file to the Python script file and click on run.

## Access input data

After succesful execution green tick mark will be displayed at the right top corner of the screen. \
To view the imported data, click the right mouse button, choose Result Dataset, Visualize option. Variuos statistics about each column are displayed. Data can be modified and filtered, missing values deleted or replaced.

## Store data to inmation

After incoming data has been shaped and machine learning module created, output can be stored back to the DataStudio. Deploy another script execution tile in ML studio, connect the tile with data output and the zip fil. Create a new item in the I/O model, copy the full property path, fill it in the example, which can be found [here](./scripts/write.py) and click on run. \
After successful run, values are set to the item in the DataStudio.

## What can your company create using Azure ML Studio and system:inmation?

We at inmation hope that the data flow easiness between system:inmation and the Azure ML Studio enables our customers to freely work with machine learning. Create your experiments and do not forget to share your successes with us.