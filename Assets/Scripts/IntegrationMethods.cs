using UnityEngine;
using System.Collections;

public class IntegrationMethods : MonoBehaviour
{
    public static void BackwardEuler(
        float h,
        Vector3 currentPosition,
        Vector3 currentVelocity,
        out Vector3 newPosition,
        out Vector3 newVelocity)
    {
        //Init acceleration
        //Gravity
        Vector3 accelerationFactor = Physics.gravity;

        //Main algorithm
        newVelocity = currentVelocity + h * accelerationFactor;

        newPosition = currentPosition + h * newVelocity;
    }

    //Euler's method - one iteration
    //Will not match Unity's physics engine
    public static void EulerForward(
        float h,
        Vector3 currentPosition,
        Vector3 currentVelocity,
        out Vector3 newPosition,
        out Vector3 newVelocity)
    {
        //Init acceleration
        //Gravity
        Vector3 accelerationFactor = Physics.gravity;
        //acceleartionFactor += CalculateDrag(currentVelocity);


        //Init velocity
        //Current velocity
        Vector3 velocityFactor = currentVelocity;
        //Wind velocity
        //velocityFactor += new Vector3(2f, 0f, 3f);


        //
        //Main algorithm
        //
        newPosition = currentPosition + h * velocityFactor;

        newVelocity = currentVelocity + h * accelerationFactor;
    }

    //Heun's method - one iteration
    //Will give a better result than Euler forward, but will not match Unity's physics engine
    //so the bullets also have to use Heuns method
    public static void Heuns(
        float h,
        Vector3 currentPosition,
        Vector3 currentVelocity,
        out Vector3 newPosition,
        out Vector3 newVelocity)
    {
        //Init acceleration
        //Gravity
        Vector3 accelerationFactorEuler = Physics.gravity;
        Vector3 accelerationFactorHeun = Physics.gravity;


        //Init velocity
        //Current velocity
        Vector3 velocityFactor = currentVelocity;
        //Wind velocity
        velocityFactor += ShootingController.windSpeed;


        //
        //Main algorithm
        //
        //Euler forward
        Vector3 pos_E = currentPosition + h * velocityFactor;

        accelerationFactorEuler += BulletPhysics.CalculateDrag(currentVelocity);

        Vector3 vel_E = currentVelocity + h * accelerationFactorEuler;


        //Heuns method
        Vector3 pos_H = currentPosition + h * 0.5f * (velocityFactor + vel_E);

        accelerationFactorHeun += BulletPhysics.CalculateDrag(vel_E);

        Vector3 vel_H = currentVelocity + h * 0.5f * (accelerationFactorEuler + accelerationFactorHeun);


        newPosition = pos_H;
        newVelocity = vel_H;
    }
}