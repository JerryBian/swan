数学知识在金融风险(FRM, Financial Risk Management)的计算中扮演着举足轻重的角色，一切的理论推演最终都以数学公式的形式展现出来。本文简单介绍一下会涉及到的数学基础知识。

### 方差(Variance)

从 [Investopedia](http://www.investopedia.com/terms/v/variance.asp) 摘抄的定义:

> The variance measures how far each number in the set is from the mean.

简单说就是：方差衡量了在一系列的数字中，个体与均值的差异。统计中的方差，是每个样本值与全体样本值的平均数只差的平方值的平均数。

方差的概念最初是在 1918 年，由 Ronald Fisher 提出。通常由 \\(\delta^2\\) 或者 Var(X) 来表示。

它的计算公式为：

$$\delta^2 = \frac{\sum (X - \mu)^2}{N}$$

其中， X 为个体变量，\\(\mu\\) 为总体均值，N 为总体个数。

### 标准差(Standard Deviation)

Quote from [Investopedia](http://www.investopedia.com/terms/s/standarddeviation.asp):

> Standard deviation is a measure of the dispersion of a set of data from its mean.

标准差描述了在一个样本集中，每一个样本值与起平均值的离散程度。平均数相同的两组数据，标准差未必相同。

标准差的公式相当简单，即是方差的平方根：

$$\delta = \sqrt {\frac{\sum (X - \mu)^2}{N}}$$

其中， X 为个体变量，\\(\mu\\) 为总体均值，N 为总体个数。

### 协方差(Covariance)

方差是衡量的一组数据的离散程度，协方差则是衡量两组数据的联系，即相互独立的程度。如果协方差为 0，则两组数据相互独立。

它的公式也相当好记，前面我们处理方差的时候由于是一组数据，方差公式可以表示成：

$$\delta^2 = \frac{\sum (X - \mu)(X - \mu)}{N}$$

协方差公式则为：

$$Cov(X,Y) = \frac{\sum (X - \mu_{X})(Y - \mu_{Y})}{N}$$

如果协方差为正，说明 X, Y 同样变化，协方差越大说明同向程度越高；如果协方差为负，说明 X, Y 反向运动，协方差越小说明反向程度越高。

由此推演开来，对于两组以上的 n 组变量，则需要一个概念叫协方差矩阵(covariance matrix)来表示。

比如对于一个三维的数据集(X,Y,Z)，协方差矩阵可以写成：

$$C = \begin{bmatrix} Cov(X,X) & Cov(X,Y) & Cov(X,Z) \\\ Cov(Y,X) & Cov(Y,Y) & Cov(Y,Z) \\\ Cov(Z,X) & Cov(Z,Y) & Cov(Z,Z) \end{bmatrix}$$

### 相关系数(Correlation coefficient)

相关系数其实可以看做标准化的协方差，它消除了两个变量变化幅度的影响，而只是单纯反应两个变量单位变化时的相似程度。

需要注意的是，我们通常所说的相关系数其实是皮尔逊相关系数(Pearson correlation coefficient)。

$$\rho = \frac{Cov(X,Y)}{\delta_{X}\delta_{Y}}$$

显而易见，\\(\rho\\) 的范围在 [-1,1] 之间，与之相对的协方差则是 \\(((-\infty),(+\infty))\\)之间。

当两组变量的相关系数为 1 时，则表明它们的正向相似度最大，为完全正相关。

当相关系数为 0 时，两组变量的变化过程没有相似度，即两个变量无关。

当相关系数为 -1 时，说明两组变量反向相似度最大，为完全负相关。