import matplotlib.pyplot as plt
from dice import Dice

dice1 = Dice()
dice2 = Dice()

rolls_sum = [dice1.roll() + dice2.roll() for _ in range(100)]

plt.hist(rolls_sum, bins=range(2, 14), align='left', rwidth=0.8)
plt.title('Distribution of Rolls Sum (2d6)')
plt.xlabel('Sum of Dice Values')
plt.ylabel('Frequency')
plt.show()

