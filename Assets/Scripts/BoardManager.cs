using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	public static BoardManager instance;
	public List<Sprite> easyFoods = new List<Sprite>();
	public List<Sprite> mediumFoods = new List<Sprite>();
	public List<Sprite> hardFoods = new List<Sprite>();
	private List<Sprite> sources = new List<Sprite>();
	public GameObject tile;
	public int xSize, ySize;

	private int difficulty = 0;
	private GameObject[,] tiles;

	public bool IsShifting { get; set; }

	void Start()
	{
		instance = GetComponent<BoardManager>();
	}

	public void StartMinigame(int dLevel)
    {
		difficulty = dLevel;
		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
		switch (difficulty)
        {
			case 1:
				sources = new List<Sprite>(easyFoods);
				break;
			case 2:
				sources = new List<Sprite>(mediumFoods);
				break;
			case 3:
				sources = new List<Sprite>(hardFoods);
				break;
			default:
				sources = new List<Sprite>(easyFoods);
				break;
		}
		CreateBoard(offset.x, offset.y);
	}



	private void CreateBoard(float xOffset, float yOffset)
	{
		tiles = new GameObject[xSize, ySize];

		float startX = transform.position.x;
		float startY = transform.position.y;

		Sprite[] previousLeft = new Sprite[ySize]; 
		Sprite previousBelow = null; 

		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
				tiles[x, y] = newTile;
				newTile.name = "X" + x + "Y" + y;
				newTile.transform.parent = transform; 
				List<Sprite> possibleCharacters = new List<Sprite>();
				possibleCharacters.AddRange(sources);
				possibleCharacters.Remove(previousLeft[y]);
				possibleCharacters.Remove(previousBelow);
				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
				newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
				previousLeft[y] = newSprite;
				previousBelow = newSprite;
			}
		}
	}

	private Sprite GetNewSprite(int x, int y)
	{
		List<Sprite> possibleFoods = new List<Sprite>();
		possibleFoods.AddRange(sources);

		if (x > 0)
		{
			possibleFoods.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
        if (x < xSize - 1)
        {
            possibleFoods.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
		{
			possibleFoods.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}
        if (y < ySize - 1)
        {
            possibleFoods.Remove(tiles[x, y + 1].GetComponent<SpriteRenderer>().sprite);
        }
        return possibleFoods[Random.Range(0, possibleFoods.Count)];
	}

	private IEnumerator MoveTilesDown(int x, int yStart, float shiftDelay = .03f)
	{
		IsShifting = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < ySize; y++)
		{
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null)
			{
				nullCount++;
			}
			renders.Add(render);
		}

		for (int i = 0; i < nullCount; i++)
		{
			GameManager.instance.PlaySFX();
			yield return new WaitForSeconds(shiftDelay);
			for (int k = 0; k < renders.Count - 1; k++)
			{
				renders[k].sprite = renders[k + 1].sprite;
				renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
			}
			if (yStart == ySize - 1)
			{
				renders[0].sprite = GetNewSprite(x, ySize - 1);
			}
		}
		IsShifting = false;
	}

	public IEnumerator FindNullTiles()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
				{
					yield return StartCoroutine(MoveTilesDown(x, y));
					break;
				}
			}
		}

		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
			}
		}
	}
}
