using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WASD to move , space to jump
/// Side notes
/// If the character is moving through objects then check if there is a collider and ensure that it is aligned to the grid
/// </summary>

public class CharacterGridController : MonoBehaviour {
    public float movementSpeed = 2.5f;

    public iTween.EaseType type = iTween.EaseType.easeInOutQuad;

    public bool useGravity = false;


    public bool isMoving = false;

    public int jumpDistance = 3;

    public bool moveForward = false;

    // The distance we go to find if there is anything below us
    private float raycastDistance = 0.5f;

    private void Awake () {
        SnapToGrid();
    }

    private void Update () {
        if ( isMoving == true )
            return;

        if ( Input.GetKey( KeyCode.W ) )
            MoveInDirection( new Vector3Int( 0 , 0 , 1 ) );
        else if ( Input.GetKey( KeyCode.S ) )
            MoveInDirection( new Vector3Int( 0 , 0 , -1 ) );
        else if ( Input.GetKey( KeyCode.A ) )
            MoveInDirection( new Vector3Int( -1 , 0 , 0 ) );
        else if ( Input.GetKey( KeyCode.D ) )
            MoveInDirection( new Vector3Int( 1 , 0 , 0 ) );
        if ( Input.GetKeyUp( KeyCode.Space ) ) {
            moveForward = true;
            MoveInDirection( new Vector3Int( 0 , jumpDistance , 0 ) );
        }
    }

    /// <summary>
    /// Used to move 1 unit in a given direction
    /// </summary>
    /// <param name="direction"></param>
    public void MoveInDirection ( Vector3 direction ) {
        if ( IsTileEmpty( direction ) ) {
            //Debug.Log( direction + transform.position + " is empty");
            isMoving = true;

            iTween.MoveTo( this.gameObject , iTween.Hash(
                "position" , direction + transform.position ,
                "speed" , movementSpeed ,
                "oncomplete" , "OnFinishMove" ,
                "easetype" , type
                )
            );

            transform.LookAt( new Vector3( direction.x , 0 , direction.z ) + transform.position );
        }
    }

    /// <summary>
    /// This is called when the itween finishes the move function, it snaps us to the grid, applies gravity and resets the isMoving variable.
    /// </summary>
    private void OnFinishMove () {
        SnapToGrid();

        if ( moveForward == true ) {
            moveForward = false;
            MoveInDirection( transform.forward );
            return;
        }

        if ( useGravity == true ) {
            // check if the space below us is empty / if it is then move down one space

            if ( IsTileEmpty( -Vector3.up ) ) {
                MoveInDirection( -Vector3.up );
                return;
            }
        }

        isMoving = false;

    }

    /// <summary>
    /// Checks if the tile in the given direction is empty, this does NOT account for movements more than one unit!
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool IsTileEmpty ( Vector3 direction ) {
        Ray r = new Ray( transform.position , direction );

        // visualise the direction we are testing for
        // Debug.DrawRay( transform.position , r.direction , Color.blue , 3.0f );

        return Physics.Raycast( r , raycastDistance ) == false;
    }

    /// <summary>
    /// Snaps the object to a grid with a resolution based on 1 x 1 x 1 tiles
    /// </summary>
    private void SnapToGrid () {
        transform.position = new Vector3Int( (int)transform.position.x , (int)transform.position.y , (int)transform.position.z );
    }

    public Color gridColor = Color.white;
    private int gridRadius = 5;

    /// <summary>
    /// This is just to better visualize the grid space, eventually this would be phased out
    /// </summary>
    private void OnDrawGizmos () {
        Gizmos.color =gridColor;

        for ( int x = -gridRadius ; x <= gridRadius ; x++ ) {
            for ( int y = -gridRadius ; y <= gridRadius ; y++ ) {
                for ( int z = -gridRadius ; z <= gridRadius ; z++ ) {
                    Gizmos.DrawWireCube( new Vector3(x , y  ,z ) + transform.position , Vector3.one );
                }
            }
        }
    }
}
