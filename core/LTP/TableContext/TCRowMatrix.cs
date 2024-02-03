using core.LTP.BTreeOnHeap;
using core.LTP.HeapNode;
using core.NDBLayer;
using core.NDBLayer.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace core.LTP.TableContext
{
    /// <summary>
    /// Row Matrix                 
    /// The Row Matrix contains the actual data for the rows and columns of the TC.The data is physically
    /// arranged in rows; each row contains the data for each of its columns.Each row of column data in the
    /// Row Matrix is of the same size and is arranged in the same layout, and the size of each row is 
    /// specified in the rgib[TCI_bm] value in the TCINFO header structure. 
    /// However, in many cases, the Row Matrix is larger than 8 kilobytes and therefore cannot fit in a single
    /// data block, which means that a data tree is used to store the Row Matrix in separate data blocks.This
    /// means that the row data is partitioned across two or more data blocks and needs special handling
    /// considerations.
    /// The design of a TC dictates that each data block MUST store an integral number of rows, which means
    /// that rows cannot span across two blocks, and that each block MUST start with a fresh row. This also
    /// means that in order for a client to access a particular row in the Row Matrix, it first calculates how
    /// many rows fit in a block, and calculates the row index within that block at which the row data is 
    /// located.The general formulas to calculate the block index and row index for the Nth row are as 
    /// follows:
    /// Rows per block = Floor((sizeof(block) – sizeof(BLOCKTRAILER)) / TCINFO.rgib[TCI_bm])
    /// Block index = N / (rows per block)
    /// Row index = N % (rows per block)
    /// Each block except the last one MUST have a size of 8192 bytes.If not, the file is considered corrupted.
    /// The size of a block is specified in the formula by sizeof(block).

    /// </summary>
    public class TCRowMatrix
    {
        public TableContext TableContext;
        public List<DataBlock> TCRMData;

        public List<TCRowData> Rows;
        public Dictionary<uint, TCRowData> RowXREF;

        public TCRowMatrix(TableContext tableContext, BTH heap)
        {
            this.Rows = new List<TCRowData>();
            this.RowXREF = new Dictionary<uint, TCRowData>();
            this.TCRMData = new List<DataBlock>();

            this.TableContext = tableContext;
            var rowMatrixHNID = this.TableContext.TCHeader.HIDRowMatrixLocation;
            if (rowMatrixHNID.hid == 0)
                return;

            if ((rowMatrixHNID.hid & 0x1F) == 0)//HID
            {
                var blockData = HeapOnNode.GetHNHIDBytes(this.TableContext.HeapOnNode, rowMatrixHNID);
                this.TCRMData.Add(new DataBlock(blockData));
            }
            else
            {
                var subNodeData = this.TableContext.HeapOnNode.Node.SubNodeData.FirstOrDefault(x => x.NodeID == rowMatrixHNID.hid);
                if (subNodeData != null)
                    this.TCRMData = subNodeData.NodeData;
                else
                {
                    var tempSubNodes = new Dictionary<ulong, NodeDataDTO>();
                    foreach (var nod in this.TableContext.HeapOnNode.Node.SubNodeData)
                        tempSubNodes.Add(nod.NodeID & 0xffffffff, nod);
                    this.TCRMData = tempSubNodes[rowMatrixHNID.hid].NodeData;
                }
            }
            var rowSize = this.TableContext.TCHeader.rgib[3];
            foreach (var row in this.TableContext.RowIndexBTH.Properties)
            {
                var rowIndex = BitConverter.ToUInt32(row.Value.Data, 0);
                var blockTrailerSize = 16;
                var maxBlockSize = 8192 - blockTrailerSize;
                var recordsPerBlock = maxBlockSize / rowSize;
                var blockIndex = (int)rowIndex / recordsPerBlock;
                var indexInBlock = rowIndex % recordsPerBlock;
                var curRow = new TCRowData(this.TCRMData[blockIndex].data, this.TableContext, heap,
                                                 (int)indexInBlock * rowSize);
                this.RowXREF.Add(BitConverter.ToUInt32(row.Key, 0), curRow);
                this.Rows.Add(curRow);
            }
        }
    }
}