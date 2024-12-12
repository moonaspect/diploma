import pandas as pd
import matplotlib.pyplot as plt

iris_url = 'https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data'
iris_columns = ['SepalLength', 'SepalWidth', 'PetalLength', 'PetalWidth', 'Species']

iris_data = pd.read_csv(iris_url, header=None, names=iris_columns)

plt.hist(iris_data['PetalLength'], bins=20, alpha=0.7, label='Petal Length')
plt.hist(iris_data['PetalWidth'], bins=20, alpha=0.7, label='Petal Width')
plt.legend()
plt.title('Distribution of Iris Petal Measurements')
plt.xlabel('Measurement')
plt.ylabel('Frequency')
plt.show()

