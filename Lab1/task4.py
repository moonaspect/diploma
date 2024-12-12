import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

#url = 'https://github.com/WillKoehrsen/Data-Analysis/raw/master/univariate_dist/data/flights.csv'
#flights_data = pd.read_csv(url)
flights_data = pd.read_csv('formatted_flights.csv')

print(flights_data.columns)

flights_data.dropna(subset=['name', 'arr_delay'], inplace=True)

plt.hist(flights_data['arr_delay'], color = 'blue', edgecolor = 'black',
         bins = int(180/5))

# Replace sns.distplot with sns.histplot
sns.histplot(flights_data['arr_delay'], bins=int(180 / 5), color='blue', edgecolor='black', kde=False)

# Add labels
plt.title('Histogram of Arrival Delays')
plt.xlabel('Delay (min)')
plt.ylabel('Frequency')
plt.show()

