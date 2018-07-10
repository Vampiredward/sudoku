using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace sudoku
{
    public partial class Form1 : Form
    {
        #region 定义结构体cell_values
        public struct cell_values
        {
            public bool preset;
            public int[] value;            
            #region 用以获取第一个数值为1的位值，包括0位
            public int get_value()
            {
                int i = 0;
                for (; i < value.Length; i++)
                {
                    if (value[i] == 1)
                    {                        
                        break;
                    }
                }

                //if (i == 10)
                //{
                //    for (int reset = 0; reset < value.Length; reset++)
                //    {
                //        value[reset] = 1;
                //    }
                //    i = 0;
                //}
                return i;
            }            
            #endregion
        }
        #endregion

        #region 定义全局公共变量
        public cell_values[,] values = new cell_values[9, 9];
        int first_none_i = 0;
        int first_none_j = 0;
        int last_none_i = 8;
        int last_none_j = 8;
        #endregion

        #region 系统创建初始化Form1函数
        public Form1()
        {
            for (int sudoku_row = 0; sudoku_row < 9; sudoku_row++)
                for (int sudoku_column = 0; sudoku_column < 9; sudoku_column++)
                {
                    this.sudoku_cells[sudoku_row, sudoku_column] = new System.Windows.Forms.TextBox();
                    this.sudoku_cells[sudoku_row, sudoku_column].Location = new System.Drawing.Point(12 + sudoku_column * 22, 12 + sudoku_row * 22);
                    this.sudoku_cells[sudoku_row, sudoku_column].Size = new System.Drawing.Size(21, 21);

                    #region 测试用初始化参数
                    //int number = (sudoku_row * 3 + sudoku_column) % 9 + sudoku_row / 3 + 1;
                    //if (number > 9)
                    //    number = number % 9;
                    //this.sudoku_cells[sudoku_row, sudoku_column].Text = number.ToString();
                    #endregion

                    this.Controls.Add(this.sudoku_cells[sudoku_row, sudoku_column]);//这句话很重要！！
                }   
            InitializeComponent();
        }
        #endregion        

        #region 系统创建button1_Click()函数
        private void button1_Click(object sender, EventArgs e)
        {
            int preset_counts = 0;
            bool row_break = false;
            bool input_valid = true;

            #region 第一步：读取预设值并确保输入的每一个字符都是有效值
            for (int sudoku_row = 0; sudoku_row < 9; sudoku_row++)
            {
                for (int sudoku_column = 0; sudoku_column < 9; sudoku_column++)
                {
                    #region 设置preset为false
                    //this.sudoku_cells[sudoku_row, sudoku_column].Enabled = false;
                    values[sudoku_row, sudoku_column].preset = false;
                    values[sudoku_row, sudoku_column].value = new int[10];
                    #endregion

                    #region 如果有有效预设值，则preset设为true，并把预设值对应位的值改成1，其余为0
                    if (sudoku_cells[sudoku_row, sudoku_column].Text.Trim() != "")
                    {
                        if (!validate_input(sudoku_cells[sudoku_row, sudoku_column].Text.Trim()))
                        {
                            row_break = true;
                            break;
                        }
                        this.sudoku_cells[sudoku_row, sudoku_column].BackColor = Color.LightBlue;
                        values[sudoku_row, sudoku_column].preset = true;
                        preset_counts++;

                        for (int i = 0; i < 10; i++)
                        {
                            if (i == Int16.Parse(this.sudoku_cells[sudoku_row, sudoku_column].Text))
                                values[sudoku_row, sudoku_column].value[i] = 1;
                            else values[sudoku_row, sudoku_column].value[i] = 0;
                        }
                    }
                    #endregion

                    #region 如果没有预设值，则preset保持为false，把每一位都改成1，get_value()函数将返回0
                    else
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            values[sudoku_row, sudoku_column].value[i] = 1;
                        }
                    }
                    #endregion
                }
                if (row_break)
                {
                    input_valid = false;
                    break;
                }
            }
            #endregion

            #region 第二步：判断预设值是否矛盾
            if (input_valid)
            {
                row_break = false;
                for (int sudoku_row = 0; sudoku_row < 9; sudoku_row++)
                {
                    for (int sudoku_column = 0; sudoku_column < 9; sudoku_column++)
                    {
                        if (values[sudoku_row, sudoku_column].preset)
                        {
                            if (!value_available(sudoku_row, sudoku_column))
                            {
                                input_valid = false;
                                row_break = true;
                                break;
                            }
                        }
                    }
                    if (row_break)
                        break;
                }
            }
            #endregion

            if (input_valid)
            {

                #region 初始化全局公共变量
                #region 读取到第一个非预设值坐标
                row_break = false;
                for (first_none_i = 0; first_none_i < 9; first_none_i++)
                {
                    for (first_none_j = 0; first_none_j < 9; first_none_j++)
                    {
                        if (!values[first_none_i, first_none_j].preset)
                        {
                            row_break = true;
                            break;
                        }
                    }
                    if (row_break)
                        break;
                }
                #endregion

                #region 读取最后一个非预设值坐标
                row_break = false;
                for (last_none_i = 8; last_none_i >= 0; last_none_i--)
                {
                    for (last_none_j = 8; last_none_j >= 0; last_none_j--)
                    {
                        if (!values[last_none_i, last_none_j].preset)
                        {
                            row_break = true;
                            break;
                        }
                    }
                    if (row_break)
                        break;
                }
                #endregion

                #region 测试用显示首尾非预设值坐标，控件已删除
                //textBox1.Text = first_none_i.ToString();
                //textBox2.Text = first_none_j.ToString();
                //textBox3.Text = last_none_i.ToString();
                //textBox4.Text = last_none_j.ToString();
                #endregion
                #endregion

                #region 调用resolve_it()函数，如果返回值为true，则调用display_it函数显示所有值，否则弹出“No resolution!”
                if (first_none_i < 9 && first_none_j < 9)
                {
                    bool resolved = resolve_it(first_none_i, first_none_j);
                    if (resolved)
                        display_it();
                    else MessageBox.Show("No resolution!");
                }
                #endregion
            }

            else MessageBox.Show("Input error!");
        }
        #endregion

        #region 自定义validate_input()函数来确保输入值都为有效值1 ~ 9
        private bool validate_input(string input)
        {
            #region 最终返回值
            bool number = true;
            #endregion

            if (input != "1" && input != "2" && input != "3" && input != "4" && input != "5" && input != "6" && input != "7" && input != "8" && input != "9")
                number = false;

            return number;
        }
        #endregion

        //每个非预设值单元格完成填写的标志有两个：
        //第一，其本身有有效值可选；
        //第二，至少存在一个有效值使其下一个非预设值单元格有有效值可选。
        #region 自定义resolve_it()函数，解题
        private bool resolve_it(int resolve_i, int resolve_j)
        {
            bool resolved = true;//最终返回值

            #region 第一步：判断该单元格本身是否有有效值可选，若否，直接返回false，跳过第二步判断
            if (!cell_available(resolve_i, resolve_j))
                resolved = false;
            #endregion

            #region 第二步：判断该单元格的下一个非预设值单元格是否有有效值可选。如果已经是最后一个非预设值单元格，则直接设值
            if (resolved)
            {
                #region 如果是最后一个非预设值，设置有效值。若无有效值，则返回false
                if (resolve_i == last_none_i && resolve_j == last_none_j)
                {
                    int _for;
                    int[] resolve_cell_values = new int[9];
                    for (_for = 0; _for < 9; _for++)
                        resolve_cell_values[_for] = values[resolve_i, resolve_j].value[_for];

                    #region 因为已经经过第一步判断是否具有有效值，作为最后一个非预设值，有且仅有一个有效值
                    values[resolve_i, resolve_j].value[0] = 0;
                    for (_for = 1; _for < 10; _for++)
                    {
                        if (value_available(resolve_i, resolve_j))
                        {
                            //values[resolve_i, resolve_j].preset = true;
                            break;
                        }
                        else values[resolve_i, resolve_j].value[_for] = 0;
                    }
                    #endregion

                    if (_for == 10)
                    {
                        resolved = false;
                        for (_for = 0; _for < 9; _for++)
                            values[resolve_i, resolve_j].value[_for] = resolve_cell_values[_for];
                    }
                }
                #endregion

                #region 如果不是最后一个非预设值，则用递归判断其下一个非预设值单元格的有效性，递归结束于最后一个非预设值单元格
                else
                {
                    Point current_id = new Point(resolve_i, resolve_j);
                    Point next_id = next_ID(current_id);
                    int next_i = next_id.X;
                    int next_j = next_id.Y;
                    
                    int _for;

                    #region 保存初始值于resolve_cell_values[]//应该不需要这个
                    int[] resolve_cell_values = new int[10];
                    for (_for = 0; _for < 10; _for++)
                        resolve_cell_values[_for] = values[resolve_i, resolve_j].value[_for];
                    #endregion

                    values[resolve_i, resolve_j].value[0] = 0;
                    for (_for = 1; _for < 10; _for++)
                    {
                        //Fucking Attention：这是整个函数最最重要的一句话，尤其是value_available(resolve_i,resolve_j)！！
                        if (value_available(resolve_i,resolve_j) && resolve_it(next_i,next_j))
                        {
                            break;
                        }
                        else values[resolve_i, resolve_j].value[_for] = 0;
                    }

                    #region 必须要有！！！如果第一个非预设值下的_for == 10，则数独无解
                    if (_for == 10)
                    {
                        for (_for = 0; _for < 10; _for++)
                            values[resolve_i, resolve_j].value[_for] = resolve_cell_values[_for];
                        resolved = false;
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            return resolved;
        }
        #endregion

        #region 自定义cell_available()函数，以判断单元格是否有有效值可选。仅在当前设定的值下判断
        private bool cell_available(int available_i, int available_j)
        {
            #region 最终返回值available
            bool available = false;
            #endregion

            int available_count = 1;
            int[] available_cell_values = new int[10];

            #region 重要！！！保存初始值于available_cell_values[]，不能直接用cell_values对象做等号负值，后续分析一下
            for (int _for = 0; _for < 10; _for++)
                available_cell_values[_for] = values[available_i, available_j].value[_for];
            //cell_values available_cell_values = values[available_i, available_j];该方法不可取！！！
            #endregion

            values[available_i, available_j].value[0] = 0;//先使该单元格的get_value()返回值不为0，然后用下面的循环测试是否具有有效值
            for (; available_count < 10; available_count++)
            {
                if (value_available(available_i, available_j))
                {
                    available = true;
                    break;
                }
                values[available_i, available_j].value[available_count] = 0;//如果该值无效，则使该位成为0，get_value()函数将得到其下一位
            }

            for (int _for = 0; _for < 10; _for++)
                values[available_i, available_j].value[_for] = available_cell_values[_for];//还原初始值

            return available;
        }
        #endregion

        #region 自定义value_available()函数，以判断当前单元格get_value()的返回值是否为有效值
        private bool value_available(int done_i, int done_j)
        {
            #region 最终返回值done
            bool done = true;
            #endregion

            #region 先check该位是否与列中某一值重复，若是直接break并且不用check行和单元九宫格，返回false
            for (int i = 0; i < 9; i++)
            {
                if (values[i, done_j].get_value() == values[done_i, done_j].get_value() && i != done_i)
                {
                    done = false;
                    break;
                }
            }
            #endregion

            #region 再check该位是否与行中某一值重复，若是直接break并且不用check单元九宫格，返回false
            if (done)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (values[done_i, j].get_value() == values[done_i, done_j].get_value() && j != done_j)
                    {
                        done = false;
                        break;
                    }
                }
            }
            #endregion

            #region 最后check该位是否与单位九宫格中某一值重复，若是直接break，返回false
            if (done)
            {
                int index_i = (done_i / 3) * 3;
                int index_j = (done_j / 3) * 3;

                for (int plus_i = 0; plus_i < 3; plus_i++)
                {
                    for (int plus_j = 0; plus_j < 3; plus_j++)
                    {
                        if (values[index_i + plus_i, index_j + plus_j].get_value() == values[done_i, done_j].get_value() && ((index_i + plus_i != done_i) || (index_j + plus_j != done_j)))
                        {
                            done = false;
                            break;
                        }
                    }
                    if (!done)
                        break;
                }
            }
            #endregion

            return done;
        }
        #endregion

        #region 自定义next_ID()函数输出下一个非预设值单元格的位置
        private Point next_ID(Point p_id)
        {
            int x = p_id.X;
            int y = p_id.Y;

            int next_x;
            int next_y;

            if (y < 8)
            {
                next_x = x;
                next_y = y + 1;
            }

            else if (x < 8)
            {
                next_x = x + 1;
                next_y = 0;
            }

            else
            {
                next_x = 0;
                next_y = 0;
            }

            Point next_id = new Point(next_x, next_y);

            if (values[next_x, next_y].preset)
                next_id = next_ID(next_id);

            return next_id;
        }
        #endregion

        #region 自定义display_it()函数，显示所有值
        private void display_it()
        {
            for (int display_it_i = 0; display_it_i < 9; display_it_i++)
                for (int display_it_j = 0; display_it_j < 9; display_it_j++)
                {
                    this.sudoku_cells[display_it_i, display_it_j].Text = values[display_it_i, display_it_j].get_value().ToString();
                }
        }
        #endregion

        #region 系统创建button2_Click()函数，清除所有值
        private void button2_Click(object sender, EventArgs e)
        {
            for (int clear_i = 0; clear_i < 9; clear_i++) 
                for (int clear_j = 0; clear_j < 9; clear_j++)
                {
                    sudoku_cells[clear_i, clear_j].Text = "";
                    sudoku_cells[clear_i, clear_j].BackColor = Color.White;
                }
        }
        #endregion

        #region 忽略：测试各自定义函数，如有需要可以添加button拷贝该段内容
        //private void button2_Click(object sender, EventArgs e)
        //{
        //    //测试value_available()代码，目测完全正确
        //    //if (value_available(8, 8))
        //    //    textBox5.Text = "Usable";
        //    //else textBox5.Text = "Not Usable";

        //    //测试cell_available()代码，目测完全正确
        //    //if (cell_available(last_none_i, last_none_j))
        //    //    textBox5.Text = "Available";
        //    //else textBox5.Text = "Not Available";

        //    //测试next_ID()代码，除了当所有值都是预设值的时候会有无限递归Exception以外完全正确，可以后期解决一下
        //    //Point current_id = new Point(0, 0);
        //    //Point next_id = next_ID(current_id);
        //    //textBox5.Text = next_id.X.ToString() + " , " + next_id.Y.ToString();
        //}
        #endregion
    }
}