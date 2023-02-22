using UnityEngine;
using MathNet.Numerics.LinearAlgebra.Single;

public class KalmanFilter
{
    DenseMatrix x_mat;              // Initial state of x
    DenseMatrix x_cov;              // Initial co-variance
    DenseMatrix state_transfer;     // Init state transfer matrix
    DenseMatrix state_covariance;   // Inti covariance matrix for state transfer
    DenseMatrix observe_matrix;
    DenseMatrix observe_noise;      // The noise of observe matrix


    public KalmanFilter()
    {
        DefaultInitMatrix();
    }

    public KalmanFilter(DenseMatrix[] _assignArray)
    {
        ReAssignMatrix(_assignArray);
    }

    public void WriteLine()
    {
        Debug.Log(x_mat);
        Debug.Log(x_cov);
        Debug.Log(state_transfer);
        Debug.Log(state_covariance);
        Debug.Log(observe_matrix);
        Debug.Log(observe_noise);
    }

    public DenseMatrix ProcessKalmanFilter(DenseMatrix _pos)
    {
        if(_pos == null)
        {
            throw new UnityException("Failed to get target position");
        }

        // parameters update
        var x_predict = state_transfer * x_mat;
        // get x covariance pridiction
        var p_predict = state_transfer * x_cov * state_transfer.Transpose();
        p_predict.Add(DenseMatrix.Create(2,2,1));
        // Claculate Kalman matrix
        var temp_denominator = observe_matrix * p_predict * observe_matrix.Transpose() + observe_noise;
        var kal = p_predict * observe_matrix.Transpose() * temp_denominator.Inverse();
        // Update x_mat: state matrix = posterior
        var pos_dif = _pos - observe_matrix * p_predict;
        var kalTimeDif = kal * pos_dif;
        if (x_predict.ColumnCount != kalTimeDif.ColumnCount) {
            x_predict = (DenseMatrix)x_predict.Append(DenseMatrix.Create(2, 1, 1));
        }
        x_mat = (DenseMatrix) (x_predict + kalTimeDif);
        // Update x state covariance matrix
        var eye = DenseMatrix.CreateDiagonal(2, 2, 1);
;       eye -= (DenseMatrix)(kal * observe_matrix);
        x_cov = (DenseMatrix)(eye * p_predict);

        return x_mat;
    }

    /********************Value Assignment methods*******************/
    /// <summary>
    /// Default settint (Value initialization) for each matrix
    /// </summary>
    private void DefaultInitMatrix()
    {
        // Assign value for each matrix
        float[,] cor_pos = new float[,] { { 0.18f }, { 8.05f} };
        x_mat = DenseMatrix.OfArray(cor_pos);

        float[,] temp_p = new float[,] { { 1, 0 }, { 0, 1 } };
        x_cov = DenseMatrix.OfArray(temp_p);

        float[,] temp_f = new float[,] { { 1, 1 }, { 0, 1 } };
        state_transfer = DenseMatrix.OfArray(temp_f);

        // Default trust the accuracy of state tranfer matrxi => small covariance
        float[,] temp_q = new float[,] { { 0.001f, 0 }, { 0, 0.001f } };
        state_covariance = DenseMatrix.OfArray(temp_q);

        float[,] temp_h = { { 1, 0 } };
        observe_matrix = DenseMatrix.OfArray(temp_h);

        observe_noise = DenseMatrix.Create(1, 1, 1);
    }

    /// <summary>
    /// Change value of each matrix using this method
    /// index: 0 ~ 5 corresponds each matrix
    /// </summary>
    /// <param name="_mats">Should exactly contains 6 element</param>
    protected void ReAssignMatrix(DenseMatrix[] _mats)
    {
        if(_mats.Length != 6)
        {
            Debug.LogError("Error length of assginment array! '_mats.Length != 6'");
            return;
        }

        x_mat = _mats[0];
        x_cov = _mats[1];
        state_transfer = _mats[2];
        state_covariance = _mats[3];
        observe_matrix = _mats[4];
        observe_noise = _mats[5];
    }
}
