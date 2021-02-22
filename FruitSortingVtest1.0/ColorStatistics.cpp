/****************************
       ucSrcImg			BGRA格式图
       nImageW,nChannelHs[3]		图像宽高	
       nCupNum				图像中的果杯数
       char cColorType		AVG_UV  AVG_H	AVG_Y	 RATIO_UV	RATIO_H		RATIO_Y 
       colorsRGB			三种颜色标记，含3个元素的数组[3]
       unColorIntervals	含3个元素的数组[3]，	若是UV空间,数组3个值均有效，且每一个按U0、V0、U1、V1顺序存于一个int
       H或若是Y空间,取数组前两个值		另H是取+180的范围值,UV是真实值右偏移128至0~255范围

       pColorCount			存储结果指针	RATIO型时，三个成员值均有效
       AVG型时，取对应前1/2个
       ****************************/
        public static void ColorStatistic24(ref byte[] bSrcImg, int nImageW,int []nChannelHs,int nCupNum,int []nLefts1,int []nLefts0,int []nLefts2, int ColorType,
           ColorRGB[] colorRGB, uint[] unColorIntervals, ref int[] ColorCount)
        {

            //MessageBox.Show("nImageW=" + nImageW.ToString() + "\nImageH=" + nImageH.ToString() + "\nCupNum=" + nCupNum.ToString() + "\nStartX = " + nStartX.ToString() + "\nnCupW = " + nCupW.ToString());

            byte ucY, ucB = 0, ucG = 0, ucR = 0;
            int nB, nG, nR, nU, nV,nOffset,nW3,nX3;
            int nH1, nH, nMinVal, nMaxVal, nLeft, nRight, nPixTotal, nTempColorCount0, nTempColorCount1, nTempColorCount2, nOkCupNum;
            uint unMinFruitGray = EXPEND1to4(DEFAULT_MINFRUITGRAY);
            try
            {
                //准确的UV边界
                byte L_V0, L_U0, L_V1, L_U1, L_V2, L_U2, L_V3, L_U3, L_V4, L_U4, L_V5, L_U5;
                byte U0 = 0, V0 = 0, U1 = 0, V1 = 0, U2 = 0, V2 = 0, U3 = 0, V3 = 0, U4 = 0, V4 = 0, U5 = 0, V5 = 0;
                if (ColorType == RATIO_UV || ColorType == AVG_UV)
                {
                    unMinFruitGray &= 0xff;
                    L_U0 = (byte)((unColorIntervals[0] & 0xFF000000) >> 24);
                    L_V0 = (byte)((unColorIntervals[0] & 0x00FF0000) >> 16);
                    L_U1 = (byte)((unColorIntervals[0] & 0x0000FF00) >> 8);
                    L_V1 = (byte)(unColorIntervals[0] & 0x000000FF);
                    if (L_U0 > L_U1)
                    {
                        U1 = L_U0;
                        U0 = L_U1;
                    }
                    else
                    {
                        U1 = L_U1;
                        U0 = L_U0;
                    }
                    if (L_V0 > L_V1)
                    {
                        V1 = L_V0;
                        V0 = L_V1;
                    }
                    else
                    {
                        V1 = L_V1;
                        V0 = L_V0;
                    }

                    L_U2 = (byte)((unColorIntervals[1] & 0xFF000000) >> 24);
                    L_V2 = (byte)((unColorIntervals[1] & 0x00FF0000) >> 16);
                    L_U3 = (byte)((unColorIntervals[1] & 0x0000FF00) >> 8);
                    L_V3 = (byte)(unColorIntervals[1] & 0x000000FF);
                    if (L_U2 > L_U3)
                    {
                        U3 = L_U2;
                        U2 = L_U3;
                    }
                    else
                    {
                        U3 = L_U3;
                        U2 = L_U2;
                    }
                    if (L_V2 > L_V3)
                    {
                        V3 = L_V2;
                        V2 = L_V3;
                    }
                    else
                    {
                        V3 = L_V3;
                        V2 = L_V2;
                    }


                    L_U4 = (byte)((unColorIntervals[2] & 0xFF000000) >> 24);
                    L_V4 = (byte)((unColorIntervals[2] & 0x00FF0000) >> 16);
                    L_U5 = (byte)((unColorIntervals[2] & 0x0000FF00) >> 8);
                    L_V5 = (byte)(unColorIntervals[2] & 0x000000FF);
                    if (L_U4 > L_U5)
                    {
                        U5 = L_U4;
                        U4 = L_U5;
                    }
                    else
                    {
                        U5 = L_U5;
                        U4 = L_U4;
                    }
                    if (L_V4 > L_V5)
                    {
                        V5 = L_V4;
                        V4 = L_V5;
                    }
                    else
                    {
                        V5 = L_V5;
                        V4 = L_V4;
                    }
                }
                else
                {
                    if (unColorIntervals[0] > unColorIntervals[1])
                    {
                        uint temp = unColorIntervals[0];
                        unColorIntervals[0] = unColorIntervals[1];
                        unColorIntervals[1] = temp;
                    }
                }


				nW3=nImageW*3;
                ColorCount[0] = 0;
                ColorCount[1] = 0;
                ColorCount[2] = 0;
                nOkCupNum = 0;
                switch (ColorType)
                {
                    case RATIO_UV:
                        {
                            for (int k = 0; k < nCupNum; k++)
                            {
								nOffset = 0;
								nTempColorCount0 = 0;
								nTempColorCount1 = 0;
								nTempColorCount2 = 0;
								nPixTotal = 0;
								//Left通道
                                for (int j = 0; j < nChannelHs[1]; j++, nOffset+= nW3)
                                {
                                    nLeft = nLefts1[k];
                                    nRight = nLefts1[k + 1];

									nX3=nLeft*3;
                                    while (nLeft < nRight)
                                    {
                                        if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
                                        {
                                            break;
                                        }
                                        nLeft++;
										nX3+=3;
                                    }

									nX3=nRight*3;
                                    while (nLeft < nRight)
                                    {
                                        if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
                                        {
                                            break;
                                        }
                                        nRight--;
										nX3-=3;
                                    }

                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                    if (nRight < nLeft)
                                    {
                                        continue;
                                    }

									nX3=nLeft*3;
                                    for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
                                    {
                                        nPixTotal++;
                                        nB = bSrcImg[nOffset+nX3];
                                        nG = bSrcImg[nOffset+nX3+1];
                                        nR = bSrcImg[nOffset+nX3+2];

                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
                                        nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU + 128);
                                        nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU + 128);

                                        nU = nU < 0 ? 0 : nU;
                                        nU = nU > 255 ? 255 : nU;
                                        nV = nV < 0 ? 0 : nV;
                                        nV = nV > 255 ? 255 : nV;
                                        if ((U4 <= nU && nU <= U5) && (V4 <= nV && nV <= V5))
                                        {
                                            if (ucY > unMinFruitGray)//注意，保留了灰度限制
                                            {
                                                nTempColorCount2++;
                                                ucB = colorRGB[2].ucB;
                                                ucG = colorRGB[2].ucG;
                                                ucR = colorRGB[2].ucR;
                                            }
                                        }
                                        else if ((U2 <= nU && nU <= U3) && (V2 <= nV && nV <= V3))
                                        {
                                            if (ucY > unMinFruitGray)
                                            {
                                                nTempColorCount1++;
                                                ucB = colorRGB[1].ucB;
                                                ucG = colorRGB[1].ucG;
                                                ucR = colorRGB[1].ucR;
                                            }
                                        }
                                        else if ((U0 <= nU && nU <= U1) && (V0 <= nV && nV <= V1))
                                        {
                                            if (ucY > unMinFruitGray)
                                            {
                                                nTempColorCount0++;
                                                ucB = colorRGB[0].ucB;
                                                ucG = colorRGB[0].ucG;
                                                ucR = colorRGB[0].ucR;
                                            }
                                        }
                                        else
                                            continue;
                                        bSrcImg[nOffset+nX3] = ucB;
                                        bSrcImg[nOffset+nX3+1] = ucG;
                                        bSrcImg[nOffset+nX3+2] = ucR;
                                    }
                                }
								//Mid通道
								for (int j = 0; j < nChannelHs[0]; j++, nOffset+= nW3)
								{
									nLeft = nLefts0[k];
									nRight = nLefts0[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
										nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU + 128);
										nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU + 128);

										nU = nU < 0 ? 0 : nU;
										nU = nU > 255 ? 255 : nU;
										nV = nV < 0 ? 0 : nV;
										nV = nV > 255 ? 255 : nV;
										if ((U4 <= nU && nU <= U5) && (V4 <= nV && nV <= V5))
										{
											if (ucY > unMinFruitGray)//注意，保留了灰度限制
											{
												nTempColorCount2++;
												ucB = colorRGB[2].ucB;
												ucG = colorRGB[2].ucG;
												ucR = colorRGB[2].ucR;
											}
										}
										else if ((U2 <= nU && nU <= U3) && (V2 <= nV && nV <= V3))
										{
											if (ucY > unMinFruitGray)
											{
												nTempColorCount1++;
												ucB = colorRGB[1].ucB;
												ucG = colorRGB[1].ucG;
												ucR = colorRGB[1].ucR;
											}
										}
										else if ((U0 <= nU && nU <= U1) && (V0 <= nV && nV <= V1))
										{
											if (ucY > unMinFruitGray)
											{
												nTempColorCount0++;
												ucB = colorRGB[0].ucB;
												ucG = colorRGB[0].ucG;
												ucR = colorRGB[0].ucR;
											}
										}
										else
											continue;
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}
								//Right通道
								for (int j = 0; j < nChannelHs[2]; j++, nOffset+= nW3)
								{
									nLeft = nLefts2[k];
									nRight = nLefts2[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
										nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU + 128);
										nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU + 128);

										nU = nU < 0 ? 0 : nU;
										nU = nU > 255 ? 255 : nU;
										nV = nV < 0 ? 0 : nV;
										nV = nV > 255 ? 255 : nV;
										if ((U4 <= nU && nU <= U5) && (V4 <= nV && nV <= V5))
										{
											if (ucY > unMinFruitGray)//注意，保留了灰度限制
											{
												nTempColorCount2++;
												ucB = colorRGB[2].ucB;
												ucG = colorRGB[2].ucG;
												ucR = colorRGB[2].ucR;
											}
										}
										else if ((U2 <= nU && nU <= U3) && (V2 <= nV && nV <= V3))
										{
											if (ucY > unMinFruitGray)
											{
												nTempColorCount1++;
												ucB = colorRGB[1].ucB;
												ucG = colorRGB[1].ucG;
												ucR = colorRGB[1].ucR;
											}
										}
										else if ((U0 <= nU && nU <= U1) && (V0 <= nV && nV <= V1))
										{
											if (ucY > unMinFruitGray)
											{
												nTempColorCount0++;
												ucB = colorRGB[0].ucB;
												ucG = colorRGB[0].ucG;
												ucR = colorRGB[0].ucR;
											}
										}
										else
											continue;
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}

								//果杯LMR均值
                                if (nPixTotal != 0)
                                {
                                    nOkCupNum++;
                                    nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;//四舍五入	
                                    nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                    nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                }

                                ColorCount[0] += nTempColorCount0;
                                ColorCount[1] += nTempColorCount1;
                                ColorCount[2] += nTempColorCount2;
                            }
                            break;
                        }
                    case AVG_UV:
                        {
							for (int k = 0; k < nCupNum; k++)
							{
								nOffset = 0;
								nTempColorCount0 = 0;
								nTempColorCount1 = 0;
								nTempColorCount2 = 0;
								nPixTotal = 0;
								//Left通道
								for (int j = 0; j < nChannelHs[1]; j++, nOffset+= nW3)
								{
									nLeft = nLefts1[k];
									nRight = nLefts1[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) + 128);
										nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) + 128);

										nU = nU < 0 ? 0 : nU;
										nU = nU > 255 ? 255 : nU;
										nV = nV < 0 ? 0 : nV;
										nV = nV > 255 ? 255 : nV;

										nTempColorCount0 += nU;
										nTempColorCount1 += nV;
									}
								}
								//Mid通道
								for (int j = 0; j < nChannelHs[0]; j++, nOffset+= nW3)
								{
									nLeft = nLefts0[k];
									nRight = nLefts0[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) + 128);
										nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) + 128);

										nU = nU < 0 ? 0 : nU;
										nU = nU > 255 ? 255 : nU;
										nV = nV < 0 ? 0 : nV;
										nV = nV > 255 ? 255 : nV;

										nTempColorCount0 += nU;
										nTempColorCount1 += nV;
									}
								}
								//Right通道
								for (int j = 0; j < nChannelHs[2]; j++, nOffset+= nW3)
								{
									nLeft = nLefts2[k];
									nRight = nLefts2[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) + 128);
										nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) + 128);

										nU = nU < 0 ? 0 : nU;
										nU = nU > 255 ? 255 : nU;
										nV = nV < 0 ? 0 : nV;
										nV = nV > 255 ? 255 : nV;

										nTempColorCount0 += nU;
										nTempColorCount1 += nV;
									}
								}

								//果杯LMR均值
								if (nPixTotal != 0)
								{
									nOkCupNum++;
									nTempColorCount0 = nTempColorCount0 / nPixTotal;//未四舍五入
									nTempColorCount1 = nTempColorCount1 / nPixTotal;
								}

								ColorCount[0] += nTempColorCount0;
								ColorCount[1] += nTempColorCount1;
							}
                        }
                        break;
                    case RATIO_Y:
                        {
							for (int k = 0; k < nCupNum; k++)
							{
								nOffset = 0;
								nTempColorCount0 = 0;
								nTempColorCount1 = 0;
								nTempColorCount2 = 0;
								nPixTotal = 0;
								//Left通道
								for (int j = 0; j < nChannelHs[1]; j++, nOffset+= nW3)
								{
									nLeft = nLefts1[k];
									nRight = nLefts1[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB + SHERU);
										if (ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
										{
											nTempColorCount0++;
											ucB = colorRGB[0].ucB;
											ucG = colorRGB[0].ucG;
											ucR = colorRGB[0].ucR;
										}
										else if (ucY < unColorIntervals[1])
										{
											nTempColorCount1++;
											ucB = colorRGB[1].ucB;
											ucG = colorRGB[1].ucG;
											ucR = colorRGB[1].ucR;
										}
										else
										{
											nTempColorCount2++;
											ucB = colorRGB[2].ucB;
											ucG = colorRGB[2].ucG;
											ucR = colorRGB[2].ucR;
										}
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}
								//Mid通道
								for (int j = 0; j < nChannelHs[0]; j++, nOffset+= nW3)
								{
									nLeft = nLefts0[k];
									nRight = nLefts0[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB + SHERU);
										if (ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
										{
											nTempColorCount0++;
											ucB = colorRGB[0].ucB;
											ucG = colorRGB[0].ucG;
											ucR = colorRGB[0].ucR;
										}
										else if (ucY < unColorIntervals[1])
										{
											nTempColorCount1++;
											ucB = colorRGB[1].ucB;
											ucG = colorRGB[1].ucG;
											ucR = colorRGB[1].ucR;
										}
										else
										{
											nTempColorCount2++;
											ucB = colorRGB[2].ucB;
											ucG = colorRGB[2].ucG;
											ucR = colorRGB[2].ucR;
										}
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}
								//Right通道
								for (int j = 0; j < nChannelHs[2]; j++, nOffset+= nW3)
								{
									nLeft = nLefts2[k];
									nRight = nLefts2[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB + SHERU);
										if (ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
										{
											nTempColorCount0++;
											ucB = colorRGB[0].ucB;
											ucG = colorRGB[0].ucG;
											ucR = colorRGB[0].ucR;
										}
										else if (ucY < unColorIntervals[1])
										{
											nTempColorCount1++;
											ucB = colorRGB[1].ucB;
											ucG = colorRGB[1].ucG;
											ucR = colorRGB[1].ucR;
										}
										else
										{
											nTempColorCount2++;
											ucB = colorRGB[2].ucB;
											ucG = colorRGB[2].ucG;
											ucR = colorRGB[2].ucR;
										}
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}

								//果杯LMR均值
								if (nPixTotal != 0)
								{
									nOkCupNum++;
									nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;//四舍五入	
									nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
									nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
								}

								ColorCount[0] += nTempColorCount0;
								ColorCount[1] += nTempColorCount1;
								ColorCount[2] += nTempColorCount2;
							}
                        }
                        break;
                    case AVG_Y:
                        {
							for (int k = 0; k < nCupNum; k++)
							{
								nOffset = 0;
								nTempColorCount0 = 0;
								nTempColorCount1 = 0;
								nTempColorCount2 = 0;
								nPixTotal = 0;
								//Left通道
								for (int j = 0; j < nChannelHs[1]; j++, nOffset+= nW3)
								{
									nLeft = nLefts1[k];
									nRight = nLefts1[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB + SHERU);
										nTempColorCount0 += ucY;
									}
								}
								//Mid通道
								for (int j = 0; j < nChannelHs[0]; j++, nOffset+= nW3)
								{
									nLeft = nLefts0[k];
									nRight = nLefts0[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB + SHERU);
										nTempColorCount0 += ucY;
									}
								}
								//Right通道
								for (int j = 0; j < nChannelHs[2]; j++, nOffset+= nW3)
								{
									nLeft = nLefts2[k];
									nRight = nLefts2[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB + SHERU);
										nTempColorCount0 += ucY;
									}
								}

								//果杯LMR均值
								if (nPixTotal != 0)
								{
									nOkCupNum++;
									nTempColorCount0 = nTempColorCount0 / nPixTotal;
								}
								ColorCount[0] += nTempColorCount0;
							}
                        }
                        break;
                    case RATIO_H:
                        {
							for (int k = 0; k < nCupNum; k++)
							{
								nOffset = 0;
								nTempColorCount0 = 0;
								nTempColorCount1 = 0;
								nTempColorCount2 = 0;
								nPixTotal = 0;
								//Left通道
								for (int j = 0; j < nChannelHs[1]; j++, nOffset+= nW3)
								{
									nLeft = nLefts1[k];
									nRight = nLefts1[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nMaxVal = nR > nG ? nR : nG;
										nMaxVal = nMaxVal > nB ? nMaxVal : nB;
										nMinVal = (nR < nG ? nR : nG);
										nMinVal = nMinVal < nB ? nMinVal : nB;
										if (nMaxVal != nMinVal)
										{
											if (nMaxVal == nR)
											{
												nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
											}
											else if (nMaxVal == nG)
											{
												nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
											}
											else
											{
												nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
											}
											nH = nH1 < 0 ? nH1 + 360 : nH1;
										}
										else
										{
											nH = 0;
										}
										nH = nH >= 360 ? 359 : nH;

										nH += 180;
										nH = nH >= 360 ? nH - 360 : nH;

										if (nH < unColorIntervals[0])
										{
											nTempColorCount0++;
											ucB = colorRGB[0].ucB;
											ucG = colorRGB[0].ucG;
											ucR = colorRGB[0].ucR;
										}
										else if (nH < unColorIntervals[1])
										{
											nTempColorCount1++;
											ucB = colorRGB[1].ucB;
											ucG = colorRGB[1].ucG;
											ucR = colorRGB[1].ucR;
										}
										else
										{
											nTempColorCount2++;
											ucB = colorRGB[2].ucB;
											ucG = colorRGB[2].ucG;
											ucR = colorRGB[2].ucR;
										}
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}
								//Mid通道
								for (int j = 0; j < nChannelHs[0]; j++, nOffset+= nW3)
								{
									nLeft = nLefts0[k];
									nRight = nLefts0[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nMaxVal = nR > nG ? nR : nG;
										nMaxVal = nMaxVal > nB ? nMaxVal : nB;
										nMinVal = (nR < nG ? nR : nG);
										nMinVal = nMinVal < nB ? nMinVal : nB;
										if (nMaxVal != nMinVal)
										{
											if (nMaxVal == nR)
											{
												nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
											}
											else if (nMaxVal == nG)
											{
												nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
											}
											else
											{
												nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
											}
											nH = nH1 < 0 ? nH1 + 360 : nH1;
										}
										else
										{
											nH = 0;
										}
										nH = nH >= 360 ? 359 : nH;

										nH += 180;
										nH = nH >= 360 ? nH - 360 : nH;

										if (nH < unColorIntervals[0])
										{
											nTempColorCount0++;
											ucB = colorRGB[0].ucB;
											ucG = colorRGB[0].ucG;
											ucR = colorRGB[0].ucR;
										}
										else if (nH < unColorIntervals[1])
										{
											nTempColorCount1++;
											ucB = colorRGB[1].ucB;
											ucG = colorRGB[1].ucG;
											ucR = colorRGB[1].ucR;
										}
										else
										{
											nTempColorCount2++;
											ucB = colorRGB[2].ucB;
											ucG = colorRGB[2].ucG;
											ucR = colorRGB[2].ucR;
										}
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}
								//Right通道
								for (int j = 0; j < nChannelHs[2]; j++, nOffset+= nW3)
								{
									nLeft = nLefts2[k];
									nRight = nLefts2[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nMaxVal = nR > nG ? nR : nG;
										nMaxVal = nMaxVal > nB ? nMaxVal : nB;
										nMinVal = (nR < nG ? nR : nG);
										nMinVal = nMinVal < nB ? nMinVal : nB;
										if (nMaxVal != nMinVal)
										{
											if (nMaxVal == nR)
											{
												nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
											}
											else if (nMaxVal == nG)
											{
												nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
											}
											else
											{
												nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
											}
											nH = nH1 < 0 ? nH1 + 360 : nH1;
										}
										else
										{
											nH = 0;
										}
										nH = nH >= 360 ? 359 : nH;

										nH += 180;
										nH = nH >= 360 ? nH - 360 : nH;

										if (nH < unColorIntervals[0])
										{
											nTempColorCount0++;
											ucB = colorRGB[0].ucB;
											ucG = colorRGB[0].ucG;
											ucR = colorRGB[0].ucR;
										}
										else if (nH < unColorIntervals[1])
										{
											nTempColorCount1++;
											ucB = colorRGB[1].ucB;
											ucG = colorRGB[1].ucG;
											ucR = colorRGB[1].ucR;
										}
										else
										{
											nTempColorCount2++;
											ucB = colorRGB[2].ucB;
											ucG = colorRGB[2].ucG;
											ucR = colorRGB[2].ucR;
										}
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}

								//果杯LMR均值
								if (nPixTotal != 0)
								{
									nOkCupNum++;
									nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;//四舍五入	
									nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
									nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
								}

								ColorCount[0] += nTempColorCount0;
								ColorCount[1] += nTempColorCount1;
								ColorCount[2] += nTempColorCount2;
							}
                        }
                        break;
                    case AVG_H:
                        {
							for (int k = 0; k < nCupNum; k++)
							{
								nOffset = 0;
								nTempColorCount0 = 0;
								nTempColorCount1 = 0;
								nTempColorCount2 = 0;
								nPixTotal = 0;
								//Left通道
								for (int j = 0; j < nChannelHs[1]; j++, nOffset+= nW3)
								{
									nLeft = nLefts1[k];
									nRight = nLefts1[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nMaxVal = nR > nG ? nR : nG;
										nMaxVal = nMaxVal > nB ? nMaxVal : nB;
										nMinVal = (nR < nG ? nR : nG);
										nMinVal = nMinVal < nB ? nMinVal : nB;
										if (nMaxVal != nMinVal)
										{
											if (nMaxVal == nR)
											{
												nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
											}
											else if (nMaxVal == nG)
											{
												nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
											}
											else
											{
												nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
											}
											nH = nH1 < 0 ? nH1 + 360 : nH1;
										}
										else
										{
											nH = 0;
										}
										nH = nH >= 360 ? 359 : nH;
										nH = nH + 180 > 360 ? nH - 180 : nH + 180;
										nTempColorCount0 += nH;
									}
								}
								//Mid通道
								for (int j = 0; j < nChannelHs[0]; j++, nOffset+= nW3)
								{
									nLeft = nLefts0[k];
									nRight = nLefts0[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nMaxVal = nR > nG ? nR : nG;
										nMaxVal = nMaxVal > nB ? nMaxVal : nB;
										nMinVal = (nR < nG ? nR : nG);
										nMinVal = nMinVal < nB ? nMinVal : nB;
										if (nMaxVal != nMinVal)
										{
											if (nMaxVal == nR)
											{
												nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
											}
											else if (nMaxVal == nG)
											{
												nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
											}
											else
											{
												nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
											}
											nH = nH1 < 0 ? nH1 + 360 : nH1;
										}
										else
										{
											nH = 0;
										}
										nH = nH >= 360 ? 359 : nH;
										nH = nH + 180 > 360 ? nH - 180 : nH + 180;
										nTempColorCount0 += nH;
									}
								}
								//Right通道
								for (int j = 0; j < nChannelHs[2]; j++, nOffset+= nW3)
								{
									nLeft = nLefts2[k];
									nRight = nLefts2[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										nPixTotal++;
										nB = bSrcImg[nOffset+nX3];
										nG = bSrcImg[nOffset+nX3+1];
										nR = bSrcImg[nOffset+nX3+2];

										nMaxVal = nR > nG ? nR : nG;
										nMaxVal = nMaxVal > nB ? nMaxVal : nB;
										nMinVal = (nR < nG ? nR : nG);
										nMinVal = nMinVal < nB ? nMinVal : nB;
										if (nMaxVal != nMinVal)
										{
											if (nMaxVal == nR)
											{
												nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
											}
											else if (nMaxVal == nG)
											{
												nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
											}
											else
											{
												nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
											}
											nH = nH1 < 0 ? nH1 + 360 : nH1;
										}
										else
										{
											nH = 0;
										}
										nH = nH >= 360 ? 359 : nH;
										nH = nH + 180 > 360 ? nH - 180 : nH + 180;
										nTempColorCount0 += nH;
									}
								}
                                if (nPixTotal != 0)
                                {
                                    nOkCupNum++;
                                    nTempColorCount0 = nTempColorCount0 / nPixTotal;
                                }
                                ColorCount[0] += nTempColorCount0;
                            }
                        }
                        break;

                    default: break;
                }

                if (nOkCupNum > 0)
                {
                    ColorCount[0] = (ColorCount[0] + (nOkCupNum >> 1)) / nOkCupNum;
                    ColorCount[1] = (ColorCount[1] + (nOkCupNum >> 1)) / nOkCupNum;
                    ColorCount[2] = (ColorCount[2] + (nOkCupNum >> 1)) / nOkCupNum;

                    int tempSum = ColorCount[0] + ColorCount[1] + ColorCount[2];
                    if (ColorType == RATIO_H || ColorType == RATIO_Y || (ColorType == RATIO_UV && (U5 - U4) * (V5 - V4) + (U3 - U2) * (V3 - V2) + (U1 - U0) * (V1 - V0) == 255 * 255))//保证加和为100
                    {
                        if (tempSum > 100)
                        {
                            int MaxVal = ColorCount[0] > ColorCount[1] ? ColorCount[0] : ColorCount[1];
                            MaxVal = MaxVal > ColorCount[2] ? MaxVal : ColorCount[2];
                            if (MaxVal == ColorCount[0])
                            {
                                ColorCount[0] = 100 - ColorCount[1] - ColorCount[2];
                            }
                            else if (MaxVal == ColorCount[1])
                            {
                                ColorCount[1] = 100 - ColorCount[0] - ColorCount[2];
                            }
                            else
                                ColorCount[2] = 100 - ColorCount[0] - ColorCount[1];
                        }
                        else if (tempSum < 100)
                        {
                            int MinVal = ColorCount[0] < ColorCount[1] ? ColorCount[0] : ColorCount[1];
                            MinVal = MinVal < ColorCount[2] ? MinVal : ColorCount[2];
                            if (MinVal == ColorCount[0])
                            {
                                ColorCount[0] = 100 - ColorCount[1] - ColorCount[2];
                            }
                            else if (MinVal == ColorCount[1])
                            {
                                ColorCount[1] = 100 - ColorCount[0] - ColorCount[2];
                            }
                            else
                                ColorCount[2] = 100 - ColorCount[0] - ColorCount[1];
                        }

                    }
                    else if (ColorType == AVG_UV)
                    {
                        ColorCount[2] = -1;
                        bool okFlag = true;
                        byte avgU = (byte)ColorCount[0];
                        byte avgV = (byte)ColorCount[1];
                        if ((U4 <= avgU && avgU <= U5) && (V4 <= avgV && avgV <= V5))
                        {
                            ucB = colorRGB[2].ucB;
                            ucG = colorRGB[2].ucG;
                            ucR = colorRGB[2].ucR;
                        }
                        else if ((U2 <= avgU && avgU <= U3) && (V2 <= avgV && avgV <= V3))
                        {
                            ucB = colorRGB[1].ucB;
                            ucG = colorRGB[1].ucG;
                            ucR = colorRGB[1].ucR;
                        }
                        else if ((U0 <= avgU && avgU <= U1) && (V0 <= avgV && avgV <= V1))
                        {
                            ucB = colorRGB[0].ucB;
                            ucG = colorRGB[0].ucG;
                            ucR = colorRGB[0].ucR;
                        }
                        else
                            okFlag = false;

                        if (okFlag)
                        {
							for (int k = 0; k < nCupNum; k++)
							{
								nOffset = 0;
								//Left通道
								for (int j = 0; j < nChannelHs[1]; j++, nOffset+= nW3)
								{
									nLeft = nLefts1[k];
									nRight = nLefts1[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
                                }
								//mid通道
								for (int j = 0; j < nChannelHs[0]; j++, nOffset+= nW3)
								{
									nLeft = nLefts0[k];
									nRight = nLefts0[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}
								//right通道
								for (int j = 0; j < nChannelHs[2]; j++, nOffset+= nW3)
								{
									nLeft = nLefts2[k];
									nRight = nLefts2[k + 1];

									nX3=nLeft*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nLeft++;
										nX3+=3;
									}

									nX3=nRight*3;
									while (nLeft < nRight)
									{
										if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
										{
											break;
										}
										nRight--;
										nX3-=3;
									}

									nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
									nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
									if (nRight < nLeft)
									{
										continue;
									}

									nX3=nLeft*3;
									for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
									{
										bSrcImg[nOffset+nX3] = ucB;
										bSrcImg[nOffset+nX3+1] = ucG;
										bSrcImg[nOffset+nX3+2] = ucR;
									}
								}
                            }
                        }

                    }
                    else if (ColorType == AVG_H || ColorType == AVG_Y)
                    {
                        ColorCount[1] = -1;
                        ColorCount[2] = -1;
                        if (ColorCount[0] < unColorIntervals[0])
                        {
                            ucB = colorRGB[0].ucB;
                            ucG = colorRGB[0].ucG;
                            ucR = colorRGB[0].ucR;
                        }
                        else if (ColorCount[0] < unColorIntervals[1])
                        {
                            ucB = colorRGB[1].ucB;
                            ucG = colorRGB[1].ucG;
                            ucR = colorRGB[1].ucR;
                        }
                        else
                        {
                            ucB = colorRGB[2].ucB;
                            ucG = colorRGB[2].ucG;
                            ucR = colorRGB[2].ucR;
                        }

						for (int k = 0; k < nCupNum; k++)
						{
							nOffset = 0;
							//Left通道
							for (int j = 0; j < nChannelHs[1]; j++, nOffset+= nW3)
							{
								nLeft = nLefts1[k];
								nRight = nLefts1[k + 1];

								nX3=nLeft*3;
								while (nLeft < nRight)
								{
									if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
									{
										break;
									}
									nLeft++;
									nX3+=3;
								}

								nX3=nRight*3;
								while (nLeft < nRight)
								{
									if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
									{
										break;
									}
									nRight--;
									nX3-=3;
								}

								nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
								nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
								if (nRight < nLeft)
								{
									continue;
								}

								nX3=nLeft*3;
								for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
								{
									bSrcImg[nOffset+nX3] = ucB;
									bSrcImg[nOffset+nX3+1] = ucG;
									bSrcImg[nOffset+nX3+2] = ucR;
								}
							}
							//mid通道
							for (int j = 0; j < nChannelHs[0]; j++, nOffset+= nW3)
							{
								nLeft = nLefts0[k];
								nRight = nLefts0[k + 1];

								nX3=nLeft*3;
								while (nLeft < nRight)
								{
									if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
									{
										break;
									}
									nLeft++;
									nX3+=3;
								}

								nX3=nRight*3;
								while (nLeft < nRight)
								{
									if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
									{
										break;
									}
									nRight--;
									nX3-=3;
								}

								nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
								nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
								if (nRight < nLeft)
								{
									continue;
								}

								nX3=nLeft*3;
								for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
								{
									bSrcImg[nOffset+nX3] = ucB;
									bSrcImg[nOffset+nX3+1] = ucG;
									bSrcImg[nOffset+nX3+2] = ucR;
								}
							}
							//right通道
							for (int j = 0; j < nChannelHs[2]; j++, nOffset+= nW3)
							{
								nLeft = nLefts2[k];
								nRight = nLefts2[k + 1];

								nX3=nLeft*3;
								while (nLeft < nRight)
								{
									if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
									{
										break;
									}
									nLeft++;
									nX3+=3;
								}

								nX3=nRight*3;
								while (nLeft < nRight)
								{
									if (bSrcImg[nOffset+nX3]>0||bSrcImg[nOffset+nX3+1]>0||bSrcImg[nOffset+nX3+2]>0) 
									{
										break;
									}
									nRight--;
									nX3-=3;
								}

								nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
								nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
								if (nRight < nLeft)
								{
									continue;
								}

								nX3=nLeft*3;
								for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
								{
									bSrcImg[nOffset+nX3] = ucB;
									bSrcImg[nOffset+nX3+1] = ucG;
									bSrcImg[nOffset+nX3+2] = ucR;
								}
							}
						}                    
                    }
                }
                else
                {
                    ColorCount[0] = -1;
                    ColorCount[1] = -1;
                    ColorCount[2] = -1;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数ColorStatistic24出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数ColorStatistic出错" + ex);
#endif
            }
        }