import matplotlib.pyplot as plt
from dice import Dice

dice = Dice()
rolls = [dice.roll() for _ in range(100)]

plt.hist(rolls, bins=range(1, 8), align='left', rwidth=0.8)
plt.title('Distribution of Rolls (1d6)')
plt.xlabel('Dice Value')
plt.ylabel('Frequency')
plt.show()
