import matplotlib.pyplot as plt
from dice import Dice

rolls_sum_3d6 = [sum([Dice().roll() for _ in range(3)]) for _ in range(100)]
rolls_sum_4d6 = [sum([Dice().roll() for _ in range(4)]) for _ in range(100)]

plt.hist(rolls_sum_3d6, bins=range(3, 19), align='left', rwidth=0.8, alpha=0.7, label='3d6')
plt.hist(rolls_sum_4d6, bins=range(4, 25), align='left', rwidth=0.8, alpha=0.7, label='4d6')
plt.legend()
plt.title('Distribution of Rolls Sum (3d6 and 4d6)')
plt.xlabel('Sum of Dice Values')
plt.ylabel('Frequency')
plt.show()
