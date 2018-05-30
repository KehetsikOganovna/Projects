Public Class Form1

    'Описание динамических массивов
    Dim a() As Integer
    Dim b() As Integer
    Dim c() As Integer
    Dim n, m As Integer

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.Close() ' Кнопка "Выход"
    End Sub

    'Формирование массива и заполнение случайными числами
    Private Sub RandomArr(n As Integer, ByRef a() As Integer)
        Dim i, x, y As Integer
        'Нижняя граница интервала возможных значений элементов массива
        x = -10 * n
        'Верхняя граница интервала возможных значений элементов массива
        y = 10 * n
        For i = 0 To n - 1
            a(i) = CInt((y - x) * Rnd()) + x
        Next i
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox3.Text = ""
        TextBox4.Text = ""
        TextBox5.Text = ""
        n = Val(TextBox1.Text) 'размер первого массива 
        m = Val(TextBox2.Text) 'размер второго массива 
        'определим размеры наших динамических массивов
        ReDim a(n - 1)
        ReDim b(m - 1)
        'Заполним первый массив случайными числами
        RandomArr(n, a)
        'Вывод второго массива
        For i = 0 To n - 1
            TextBox3.Text = TextBox3.Text & Str(a(i)) & " "
        Next i
        'Заполним второй массив случайными числами
        RandomArr(m, b)
        'Вывод второго массива
        For i = 0 To m - 1
            TextBox4.Text = TextBox4.Text & Str(b(i)) & " "
        Next i
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Размер объединенного массива n + m
        ReDim c(n + m)
        'В начале массива с() сначала следуют элементы массива а()
        For i = 0 To n - 1
            c(i) = a(i)
        Next i
        ' А затем  массива b()
        For i = n To (n + m) - 1
            c(i) = b(i - n)
        Next i
        'Вывод на TextBox
        For i = 0 To (n + m) - 1
            TextBox5.Text = TextBox5.Text & Str(c(i)) & " "
        Next i
    End Sub

    'Решение Задачи1: "1. Найти сумму положительных элементов, значения которых,  состоят из двух цифр."
    Private Function SumPositDouble(k As Integer, a() As Integer) As Integer
        SumPositDouble = 0
        For i = 0 To k - 1
            'Если число положительное и двузначное, то добавить к сумме
            If (a(i) > 0) And (a(i) > 10) And (a(i) < 100) Then
                SumPositDouble = SumPositDouble + a(i)
            End If
        Next i
    End Function

    'Число v кратно k
    Private Function Multiple(v As Integer, k As Integer) As Boolean
        ' Если полное частное минус целочисленное частное = 0, то делит нацело
        Multiple = (v / k - v \ k = 0)
    End Function

    'Решение Задачи2: "2. Найти количество тех элементов, значения которых по модулю превосходят 10 и кратны 5 и 10."
    Private Function CountMore10(k As Integer, a() As Integer) As Integer
        CountMore10 = 0
        For i = 0 To k - 1
            'Если модуль числа больше 10 и кратно 5 и 10, то увеличить счётчик на 1
            If (Math.Abs(a(i)) > 10) And Multiple(a(i), 5) And Multiple(a(i), 10) Then
                CountMore10 = CountMore10 + 1
            End If
        Next i
    End Function

    Function Swap(ByRef x As Integer, ByRef y As Integer)
        Dim v As Integer
        v = x
        x = y
        y = v
    End Function

    'Решение Задачи3: "3. Поменять местами 2, 4 и 6 элемент с тремя последними элементами, сохраняя порядок их следования."
    Private Function Swap246(k As Integer, ByRef a() As Integer) As Integer()
        If k < 6 Then
            Exit Function
        End If
        Swap(a(1), a(k - 3)) 'второй элемент это a(1), т.к. отсчёт идёт с 0
        Swap(a(3), a(k - 2)) 'предпоследний элемент это a(k - 2), т.к. последний элемент это a(k - 1)
        Swap(a(5), a(k - 1)) 'последний элемент это a(k - 1), т.к. отсчёт идёт с 0
        Swap246 = a
    End Function

    'Печать массива
    Private Function ShowArr(k As Integer, a() As Integer) As String
        If k < 6 Then
            ShowArr = "Ошибка: массив должен быть размером больше 6"
            Exit Function
        End If
        ShowArr = ""
        For i = 0 To k - 1
            ShowArr = ShowArr & a(i) & " "
        Next i
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Если выбрана первая задача (четвертая кнопка)
        If RadioButton4.Checked() Then
            'Если нажата первая кнопка (выбран для работы первый массив)
            If RadioButton1.Checked() Then
                TextBox7.Text = SumPositDouble(n, a)
            End If
            'Если нажата вторая кнопка (выбран для работы второй массив)
            If RadioButton2.Checked() Then
                TextBox7.Text = SumPositDouble(m, b)
            End If
            'Если нажата третья кнопка (выбран для работы третий массив)
            If RadioButton3.Checked() Then
                TextBox7.Text = SumPositDouble(n + m, c)
            End If
        End If


        'Если выбрана вторая задача (пятая кнопка)
        If RadioButton5.Checked() Then
            'Если нажата первая кнопка (выбран для работы первый массив)
            If RadioButton1.Checked() Then
                TextBox7.Text = CountMore10(n, a)
            End If
            'Если нажата первая кнопка (выбран для работы первый массив)
            If RadioButton2.Checked() Then
                TextBox7.Text = CountMore10(m, b)
            End If
            'Если нажата первая кнопка (выбран для работы первый массив)
            If RadioButton3.Checked() Then
                TextBox7.Text = CountMore10(n + m, c)
            End If
        End If


        'Если выбрана третья задача (шестая кнопка)
        If RadioButton6.Checked() Then
            'Если нажата первая кнопка (выбран для работы первый массив)
            If RadioButton1.Checked() Then
                TextBox7.Text = ShowArr(n, Swap246(n, a))
            End If
            'Если нажата первая кнопка (выбран для работы первый массив)
            If RadioButton2.Checked() Then
                TextBox7.Text = ShowArr(m, Swap246(m, b))
            End If
            'Если нажата первая кнопка (выбран для работы первый массив)
            If RadioButton3.Checked() Then
                TextBox7.Text = ShowArr(n + m, Swap246(n + m, c))
            End If
        End If
    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged

    End Sub
End Class
